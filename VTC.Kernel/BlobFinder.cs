using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using VTC.Kernel;

namespace VTC.Kernel
{
    public class BlobFinder
    {
        
        public SortedList<CvBlob, int> FindBlobs(Image<Gray, byte> movementMask, int minSize)
        {
            var resultingImgBlobs = new CvBlobs();
            var bDetect = new CvBlobDetector();
            bDetect.Detect(movementMask, resultingImgBlobs);

            var areaComparer = new BlobAreaComparer();
            var blobsWithArea = new SortedList<CvBlob, int>(areaComparer);
            foreach (var targetBlob in resultingImgBlobs.Values.Where(targetBlob => targetBlob.Area > minSize))
                blobsWithArea.Add(targetBlob, targetBlob.Area);

            return blobsWithArea;
        }


        public static Measurements[] SplitAndFindCenterpoints(Image<Bgr,byte> maskedBlobsImage, Image<Gray,byte> blobsImage, CvBlob cvblob)
        {
            throw new NotImplementedException();    //This function sucks, needs improvement. The intention is to split overlapping blobs but it doesn't work well.

            int numMeasurements = Subitize(maskedBlobsImage, blobsImage, cvblob);
            Measurements[] measurements;
            if (numMeasurements > 1)
            {
                //Feed pixels (x,y,r,g,b) into mixture model as measurements
                List<int[]> samplesList = new List<int[]>();
                var mblobs_roi_orig = maskedBlobsImage.ROI;
                var blobs_roi_orig = blobsImage.ROI;

                maskedBlobsImage.ROI = cvblob.BoundingBox;
                blobsImage.ROI = cvblob.BoundingBox;
                int downsampleFactor = 5;
                for (int i = 0; i < cvblob.BoundingBox.Width; i+= downsampleFactor)
                    for (int j = 0; j < cvblob.BoundingBox.Height; j += downsampleFactor)
                    {
                        int x = cvblob.BoundingBox.X + i;
                        int y = cvblob.BoundingBox.Y + j;
                        if (blobsImage.Data[y,x,0] > 0)
                        {
                            //int[] sample = new[] { x, y, maskedBlobsImage.Data[y, x, 0],maskedBlobsImage.Data[y, x, 1],maskedBlobsImage.Data[y, x, 2]};
                            int[] sample = new[] { x, y, 1, 1, 1 };
                            samplesList.Add(sample);
                        }
                    }

                maskedBlobsImage.ROI = mblobs_roi_orig;
                blobsImage.ROI = blobs_roi_orig;
                var rnd = new Random();
                //int[][] samples = samplesList.OrderBy(item=>rnd.Next()).ToArray();
                int[][] samples = samplesList.ToArray();
                MixtureModel mm = new MixtureModel(samples, numMeasurements, 10);
                
                mm.Train();

                measurements = new Measurements[numMeasurements];
                for (int i = 0; i < numMeasurements; i++)
                {
                    Measurements m = new Measurements();
                    m.X = mm.Means[i][0];
                    m.Y = mm.Means[i][1];
                    m.Red = maskedBlobsImage.Data[(int)m.Y,(int)m.X,2];
                    m.Green = maskedBlobsImage.Data[(int)m.Y, (int)m.X, 1];
                    m.Blue = maskedBlobsImage.Data[(int)m.Y, (int)m.X, 0];
                    
                    measurements[i] = m;
                }
            }
            else
            {
                Measurements m = new Measurements();
                m.X = cvblob.Centroid.X;
                m.Y = cvblob.Centroid.Y;
                m.Red = maskedBlobsImage.Data[(int)m.Y, (int)m.X, 2];
                m.Green = maskedBlobsImage.Data[(int)m.Y, (int)m.X, 1];
                m.Blue = maskedBlobsImage.Data[(int)m.Y, (int)m.X, 0];
                measurements = new Measurements[1] {m};
            }

            return measurements;
        }

        const int WidthSplitThreshold = 50;
        const int HeightSplitThreshold = 50; 
        public static int Subitize(Image<Bgr,byte> maskedBlob, Image<Gray,byte> blob, CvBlob cvblob )
        {
            if (cvblob.BoundingBox.Width > WidthSplitThreshold && cvblob.BoundingBox.Height > HeightSplitThreshold)
                return 2;

            return 1;
        }
    }
}
