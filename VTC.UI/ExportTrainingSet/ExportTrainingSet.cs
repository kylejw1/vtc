using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using VTC.Kernel;
using VTC.Common;

namespace VTC.ExportTrainingSet
{
    public class ExportTrainingSet
    {
        private readonly AppSettings _settings;
        private Image<Bgr, float> _frame;
        private Image<Gray, byte> _movementMask;
        private PictureBox _picture = new PictureBox();
        private PictureBox _subimage = new PictureBox();
        private List<RadioButton> classRadioButtons = new List<RadioButton>();
        private List<Vehicle> _vehicles;

        private int _unpaddedX;
        private int _unpaddedY;

        public ExportTrainingSet(AppSettings settings, Image<Bgr, float> bgImage, List<Vehicle> currentVehicles,
            Image<Gray, byte> movementMask)
        {
            _settings = settings;
            _vehicles = new List<Vehicle>();
            foreach (Vehicle v in currentVehicles)
                _vehicles.Add(v);

            _unpaddedX = bgImage.Width;
            _unpaddedY = bgImage.Height;

            int paddedWidth = bgImage.Width + _settings.ClassifierSubframeWidth;
            int paddedHeight = bgImage.Height + _settings.ClassifierSubframeHeight;
            _frame = new Image<Bgr, float>(paddedWidth, paddedHeight);
            _movementMask = new Image<Gray, byte>(paddedWidth, paddedHeight);
            CopySubimage(bgImage, _frame);
            CopySubimage(movementMask, _movementMask);


            _picture.Width = _frame.Width;
            _picture.Height = _frame.Height;
            _picture.Image = _frame.ToBitmap();
        }

        private void CopySubimage(Image<Bgr, float> source, Image<Bgr, float> destination)
        {
            int x_offset = (destination.Width - source.Width)/2 - 1;
            int y_offset = (destination.Height - source.Height)/2 - 1;
            for (int i = 0; i < source.Width; i++)
                for (int j = 0; j < source.Height; j++)
                {
                    destination.Data[j + y_offset, i + x_offset, 0] = source.Data[j, i, 0];
                    destination.Data[j + y_offset, i + x_offset, 1] = source.Data[j, i, 1];
                    destination.Data[j + y_offset, i + x_offset, 2] = source.Data[j, i, 2];
                }
        }

        private void CopySubimage(Image<Gray, byte> source, Image<Gray, byte> destination)
        {
            int x_offset = (destination.Width - source.Width)/2 - 1;
            int y_offset = (destination.Height - source.Height)/2 - 1;
            for (int i = 0; i < source.Width; i++)
                for (int j = 0; j < source.Height; j++)
                {
                    destination.Data[j + y_offset, i + x_offset, 0] = source.Data[j, i, 0];
                }
        }

        private Image<Bgr, float> extractSubImage(CvBlob blob)
        {
            Debug.WriteLine("Exporting image with blob info:" );
            Debug.WriteLine(blob.Centroid.ToString());
            Debug.WriteLine(blob.BoundingBox.ToString());
            Image<Bgr, float> subimage = _frame.GetSubRect(blob.BoundingBox);
            return subimage;
        }

        private Image<Bgr, float> extractScaledSubImage(CvBlob blob)
        {
            int subimageWidth = _settings.ClassifierSubframeWidth;
            int subimageHeight = _settings.ClassifierSubframeHeight;

            Image<Bgr, float> subimageUnscaled = _frame.GetSubRect(blob.BoundingBox);
            Image<Bgr, float> subimage = subimageUnscaled.Resize(subimageWidth, subimageHeight, Inter.Area);

            return subimage;
        }

        private Image<Gray, byte> extractScaledSubMask(CvBlob blob)
        {
            int subimageWidth = _settings.ClassifierSubframeWidth;
            int subimageHeight = _settings.ClassifierSubframeHeight;

            Image<Gray, byte> subimageUnscaled = _movementMask.GetSubRect(blob.BoundingBox);
            Image<Gray, byte> subimage = subimageUnscaled.Resize(subimageWidth, subimageHeight, Inter.Area);

            return subimage;
        }

        private Image<Gray, byte> extractSubMask(CvBlob blob)
        {
            Image<Gray, byte> subimageUnscaled = _movementMask.GetSubRect(blob.BoundingBox);
            return subimageUnscaled;
        }

        private void saveExampleImage(Image<Bgr, float> image, string classString)
        {
            string examplePath = constructExamplePath(classString);
            image.Save(examplePath);
        }

        private void saveExampleImage(Image<Gray, byte> image, string classString)
        {
            string examplePath = constructExamplePath(classString);
            image.Save(examplePath);
        }

        private static string constructExamplePath(string classString)
        {
            string examplesDirectory = createExamplesDirectoryIfNotExists();
            string classDirectory = createClassDirectoryIfNotExists(classString, examplesDirectory);
            List<Int64> filenamesToNumbers =
                Directory.GetFiles(classDirectory)
                    .ToList<String>()
                    .Select(s => Convert.ToInt64(System.IO.Path.GetFileNameWithoutExtension(s)))
                    .ToList();
            filenamesToNumbers.Add(0);
            filenamesToNumbers.Sort();
            var newExampleNum = filenamesToNumbers.Last() + 1;
            string examplePath = classDirectory + "\\" + newExampleNum + ".bmp";
            return examplePath;
        }

        private static string createClassDirectoryIfNotExists(string classString, string examplesDirectory)
        {
            string classDirectory = examplesDirectory + "\\" + classString;
            Boolean classDirectoryExists = Directory.Exists(classDirectory);
            if (!classDirectoryExists)
                Directory.CreateDirectory(classDirectory);

            return classDirectory;
        }

        private static string createExamplesDirectoryIfNotExists()
        {
            string examplesDirectory = Directory.GetCurrentDirectory() + "\\examples";
            Boolean examplesDirectoryExists = Directory.Exists(examplesDirectory);
            if (!examplesDirectoryExists)
                Directory.CreateDirectory(examplesDirectory);
            return examplesDirectory;
        }

        public void autoExportScaledPositives()
        {
            //TODO: update commented code to call blob-finding directly, to avoid AccessViolationException
            throw new NotImplementedException();

            //var bf = new BlobFinder();
            //var blobsWithSizes = bf.FindBlobs(_movementMask, _settings.MinObjectSize);

            //foreach (var blobWithArea in blobsWithSizes)
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //foreach (var blobWithArea in blobsWithSizes)
            //{
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //    if (blobWithArea.Key.Area > 10000 ||
            //        blobWithArea.Key.Centroid.X > 640 | blobWithArea.Key.Centroid.Y > 480)
            //        Console.WriteLine("Bad blob!");

            //    Image<Bgr, float> subimage = extractScaledSubImage(blobWithArea.Key);
            //    saveExampleImage(subimage, "Car"); //TODO: Make path a configuration item
            //}
        }

        public void autoExportScaledMasks()
        {
            //TODO: update commented code to call blob-finding directly, to avoid AccessViolationException
            throw new NotImplementedException();

            //var bf = new BlobFinder();
            //var blobsWithSizes = bf.FindBlobs(_movementMask, _settings.MinObjectSize);

            //foreach (var blobWithArea in blobsWithSizes)
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //foreach (var blobWithArea in blobsWithSizes)
            //{
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //    if (blobWithArea.Key.Area > 10000 ||
            //        blobWithArea.Key.Centroid.X > 640 | blobWithArea.Key.Centroid.Y > 480)
            //        Console.WriteLine("Bad blob!");

            //    Image<Gray, byte> subimage = extractScaledSubMask(blobWithArea.Key);
            //    saveExampleImage(subimage, "Car"); //TODO: Make path a configuration item
            //}
        }

        public void autoExportMasks()
        {
            //TODO: update commented code to call blob-finding directly, to avoid AccessViolationException
            throw new NotImplementedException();

            //var bf = new BlobFinder();
            //var blobsWithSizes = bf.FindBlobs(_movementMask, _settings.MinObjectSize);

            //foreach (var blobWithArea in blobsWithSizes)
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //foreach (var blobWithArea in blobsWithSizes)
            //{
            //    if (Single.IsNaN(blobWithArea.Key.Centroid.X) || Single.IsNaN(blobWithArea.Key.Centroid.Y))
            //        Console.WriteLine("Bad blob!");

            //    if (blobWithArea.Key.Area > 10000 ||
            //        blobWithArea.Key.Centroid.X > 640 | blobWithArea.Key.Centroid.Y > 480)
            //        Console.WriteLine("Bad blob!");

            //    Image<Gray, byte> subimage = extractSubMask(blobWithArea.Key);
            //    saveExampleImage(subimage, "Car"); //TODO: Make path a configuration item
            //}
        }

        public void autoExportDualImages()
        {
            //var bf = new BlobFinder();
            //var blobsWithSizes = bf.FindBlobs(_movementMask, _settings.MinObjectSize);

            //The blob-detection code needs to be called directly in the scope of whatever function uses the blobs, due to issues in Emgu/OpenCV
            //where the blobs are garbage-detected if the original set of CvBlobs is disposed.

            var resultingImgBlobs = new CvBlobs();
            var bDetect = new CvBlobDetector();
            bDetect.Detect(_movementMask, resultingImgBlobs);

            var areaComparer = new BlobAreaComparer();
            var blobsWithArea = new SortedList<CvBlob, int>(areaComparer);
            foreach (var targetBlob in resultingImgBlobs.Values.Where(targetBlob => targetBlob.Area > _settings.MinObjectSize))
                blobsWithArea.Add(targetBlob, targetBlob.Area);

            foreach (var blobWithArea in blobsWithArea)
            {
                bool bad = false;

                if (blobWithArea.Key.BoundingBox.Width <= 0 || blobWithArea.Key.BoundingBox.Width >= 640)
                    bad = true;

                if (blobWithArea.Key.BoundingBox.Height <= 0 || blobWithArea.Key.BoundingBox.Height >= 480)
                    bad = true;

                if (blobWithArea.Key.BoundingBox.X <= 0 || blobWithArea.Key.BoundingBox.X >= 640)
                    bad = true;

                if (blobWithArea.Key.BoundingBox.Y <= 0 || blobWithArea.Key.BoundingBox.Y >= 480)
                    bad = true;

                if (!bad)
                {
                    Image<Bgr, float> subimage2 = extractSubImage(blobWithArea.Key);
                    Image<Bgr, float> subimage = extractSubMask(blobWithArea.Key).Convert<Bgr, float>();
                    Image<Bgr, float> joined = new Image<Bgr, float>(
                    subimage.Width + subimage2.Width, subimage.Height);
                    joined.ROI = new Rectangle(0, 0, subimage.Width, subimage.Height);
                    subimage.CopyTo(joined);
                    joined.ROI = new Rectangle(subimage.Width, 0, subimage2.Width, subimage.Height);
                    subimage2.CopyTo(joined);
                    joined.ROI = new Rectangle(0, 0, subimage.Width + subimage2.Width, joined.Height);

                    saveExampleImage(joined, "Car"); //TODO: Make path a configuration item    
                }
                else
                    Debug.WriteLine("Skipped export on bad bounding box.");
            }
        }
    }
}
    

