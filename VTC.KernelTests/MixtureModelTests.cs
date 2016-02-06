using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTC.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace VTC.Kernel.Tests
{
    [TestClass()]
    public class MixtureModelTests
    {
        [TestMethod()]
        public void MixtureModelTest()
        {
            var mm = new MixtureModel();
            mm.TrainIncremental(new int[] {50, 20, 10});

            //Distribution is initialized correctly from the first sample
            var background = mm.SampleBackground();
            Assert.IsTrue(background[0] == 50);
            Assert.IsTrue(background[1] == 20);
            Assert.IsTrue(background[2] == 10);

            mm.TrainIncremental(new int[] { 51, 21, 11 });
            mm.TrainIncremental(new int[] { 52, 22, 12 });
            mm.TrainIncremental(new int[] { 53, 23, 13 });
            mm.TrainIncremental(new int[] { 51, 21, 11 });
            mm.TrainIncremental(new int[] { 52, 22, 12 });
            mm.TrainIncremental(new int[] { 53, 23, 13 });
            mm.TrainIncremental(new int[] { 51, 21, 11 });
            mm.TrainIncremental(new int[] { 52, 22, 12 });
            mm.TrainIncremental(new int[] { 53, 23, 13 });
            background = mm.SampleBackground();

            //Background is updated from similar incoming samples
            Assert.IsTrue(background[0] > 50 && background[0] < 53);
            Assert.IsTrue(background[1] > 20 && background[1] < 23);
            Assert.IsTrue(background[2] > 10 && background[2] < 13);

            mm.TrainIncremental(new int[] { 250, 250, 250 });
            mm.TrainIncremental(new int[] { 250, 250, 250 });
            mm.TrainIncremental(new int[] { 250, 250, 250 });
            background = mm.SampleBackground();

            //Background is not perturbed by dissimilar incoming (foreground) samples
            Assert.IsTrue(background[0] > 50 && background[0] < 53);
            Assert.IsTrue(background[1] > 20 && background[1] < 23);
            Assert.IsTrue(background[2] > 10 && background[2] < 13);
        }
    }
}
