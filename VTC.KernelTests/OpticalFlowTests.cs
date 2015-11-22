using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTC.Kernel;
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
    }
}
