using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;


namespace DNNClassifier
{
    class RBMDataConverter
    {
        public double[] ImagePathToInput(string imagePath)
        {
            var im = new Image<Bgr, byte>(imagePath);
            var data = new double[im.Data.Length + 1];
            data[0] = 1; // Bias
            var index = 1;
            for(var x=0;x<im.Width; x++)
                for (var y = 0; y < im.Height; y++)
                {
                    data[index++] = (double) im.Data[x, y, 0] / byte.MaxValue;
                    data[index++] = (double) im.Data[x, y, 1] / byte.MaxValue;
                    data[index++] = (double) im.Data[x, y, 2] / byte.MaxValue;
                }
            
            return data;
        }

        public double[][] TrainingSetFromPath(string folderPath)
        {
            var paths = Directory.GetFiles(folderPath);
            
            var trainingSets = new double[paths.Length][];
            for (var i = 0; i < paths.Length; i++)
                trainingSets[i] = ImagePathToInput(paths[i]);
            
            return trainingSets;
        }

        public void SaveDataToImage(double[] output, string imagePath, int height, int width)
        {
            var index = 1; //Skip the first data point, it's the bias
            var im = new Image<Bgr, byte>(new Size(width, height));
            for(var x=0;x<height;x++)
                for (var y = 0; y < height; y++)
                {
                    var r = (byte.MaxValue)*output[index++];
                    var g = (byte.MaxValue)*output[index++];
                    var b = (byte.MaxValue)*output[index++];

                    r = (r > 255) ? 255 : r;
                    r = (r < 0.0) ? 0 : r;

                    g = (g > 255) ? 255 : g;
                    g = (g < 0.0) ? 0 : g;

                    b = (b > 255) ? 255 : b;
                    b = (b < 0.0) ? 0 : b;

                    im.Data[x, y, 0] = Convert.ToByte(r);
                    im.Data[x, y, 1] = Convert.ToByte(g);
                    im.Data[x, y, 2] = Convert.ToByte(b); 
                }

            im.Save(imagePath);
        }

        public void SaveRawDataToImage(double[] output, string imagePath, int height, int width)
        {
            var index = 1; //Skip the first data point, it's the bias
            var im = new Image<Bgr, byte>(new Size(width, height));
            for (var x = 0; x < height; x++)
                for (var y = 0; y < height; y++)
                {
                    var r = output[index++];
                    var g = output[index++];
                    var b = output[index++];

                    r = (r > 255) ? 255 : r;
                    r = (r < 0.0) ? 0 : r;

                    g = (g > 255) ? 255 : g;
                    g = (g < 0.0) ? 0 : g;

                    b = (b > 255) ? 255 : b;
                    b = (b < 0.0) ? 0 : b;

                    im.Data[x, y, 0] = Convert.ToByte(r);
                    im.Data[x, y, 1] = Convert.ToByte(g);
                    im.Data[x, y, 2] = Convert.ToByte(b);
                }

            im.Save(imagePath);
        }
    }
}
