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
            int numSamples = 10000;
            double meanOne = 15;
            double meanTwo = 100;
            double errorThreshold = 3.0;
            Normal componentOne = new Normal(meanOne, 5);
            Normal componentTwo = new Normal(meanTwo, 25);
            Random r = new Random();
            double weightOne = r.NextDouble();
            double weightTwo = 1 - weightOne;
            int[][] samples = new int[numSamples][];
            
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = new int[3];
                if (r.NextDouble() > weightOne)
                {
                    samples[i][0] = Convert.ToInt32(Math.Round(componentTwo.Sample()));
                    samples[i][1] = Convert.ToInt32(Math.Round(componentTwo.Sample()));
                    samples[i][2] = Convert.ToInt32(Math.Round(componentTwo.Sample()));   
                }
                else
                {
                    samples[i][0] = Convert.ToInt32(Math.Round(componentOne.Sample()));
                    samples[i][1] = Convert.ToInt32(Math.Round(componentOne.Sample()));
                    samples[i][2] = Convert.ToInt32(Math.Round(componentOne.Sample()));
                }
            }

            MixtureModel mix = new MixtureModel(samples);
            mix.Train();

            double meansError = Math.Min(Math.Abs(mix.Means[0][0] - meanOne), Math.Abs(mix.Means[0][0] - meanTwo));

            Assert.IsTrue(meansError < errorThreshold);

        }
    }
}
