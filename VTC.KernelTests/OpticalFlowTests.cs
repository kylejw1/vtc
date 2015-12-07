using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VTC.Kernel;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VTC.Kernel.Tests
{
    [TestClass()]
    public class OpticalFlowTests
    {
        [TestMethod()]
        public void SumSquareErrorTest()
        {
            int[] a = {0, 0, 0};
            int[] b = {0, 0, 0};
            double sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 0.0);

            a[0] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 1.0);

            b[0] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 0.0);

            a[0] = 5;
            a[1] = 7;
            a[2] = 9;
            b[0] = 4;
            b[1] = 5;
            b[2] = 6;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 14.0);
        }

        [TestMethod()]
        public void SumSquareErrorTest2D()
        {
            int[][] a = new int[2][];
            int[][] b = new int[2][];

            for (int i = 0; i < 2; i++)
            {
                a[i] = new int[2];
                b[i] = new int[2];
            }

            double sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 0.0);

            a[0][0] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 1.0);

            b[0][0] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 0.0);

            a[0][1] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 1.0);

            b[0][1] = 1;
            sse = OpticalFlow.SumSquareError(a, b);
            Assert.AreEqual(sse, 0.0);
        }

        /// <summary>
        /// Prepare synthetic image for testing
        /// </summary>
        /// <param name="prevImg"></param>
        /// <param name="currImg"></param>
        public static void OpticalFlowImage(out Image<Gray, Byte> prevImg, out Image<Gray, Byte> currImg)
        {
            //Create a random object
            Image<Gray, Byte> randomObj = new Image<Gray, byte>(50, 50);
            randomObj.SetRandUniform(new MCvScalar(), new MCvScalar(255));

            //Draw the object in image1 center at (100, 100);
            prevImg = new Image<Gray, byte>(300, 200);
            Rectangle objectLocation = new Rectangle(75, 75, 50, 50);
            prevImg.ROI = objectLocation;
            randomObj.Copy(prevImg, null);
            prevImg.ROI = Rectangle.Empty;

            //Draw the object in image2 center at (102, 103);
            currImg = new Image<Gray, byte>(300, 200);
            objectLocation.Offset(2, 3);
            currImg.ROI = objectLocation;
            randomObj.Copy(currImg, null);
            currImg.ROI = Rectangle.Empty;
        }

        [TestMethod()]
        public void TestCudaBroxOpticalFlow()
        {
            if (!CudaInvoke.HasCuda)
                return;
            Image<Gray, Byte> prevImg, currImg;
            OpticalFlowImage(out prevImg, out currImg);
            Mat flow = new Mat();
            CudaBroxOpticalFlow opticalflow = new CudaBroxOpticalFlow();
            using (CudaImage<Gray, float> prevGpu = new CudaImage<Gray, float>(prevImg.Convert<Gray, float>()))
            using (CudaImage<Gray, float> currGpu = new CudaImage<Gray, float>(currImg.Convert<Gray, float>()))
            using (GpuMat flowGpu = new GpuMat())
            {
                opticalflow.Calc(prevGpu, currGpu, flowGpu);
                flowGpu.Download(flow);
            }

            var channels = flow.Split();
            var xflow = new Image<Gray, float>(channels[0].Bitmap);
            var yflow = new Image<Gray, float>(channels[1].Bitmap);

            var ch1 = channels[0];
            var ch2 = channels[1];
            float[, ,] xarr = new float[xflow.Height, xflow.Width, 1];
            ch1.CopyTo(xarr);
            float[, ,] yarr = new float[xflow.Height, xflow.Width, 1];
            ch2.CopyTo(yarr);

            (new ImageViewer(prevImg)).ShowDialog();
            (new ImageViewer(currImg)).ShowDialog();

            (new ImageViewer(xflow)).ShowDialog();
            (new ImageViewer(yflow)).ShowDialog();
        }
    }
}
