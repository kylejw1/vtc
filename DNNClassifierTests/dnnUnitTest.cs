using System;
using DNNClassifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNNClassifierTests
{
    [TestClass]
    public class DnnUnitTest
    {
        [TestMethod]
        public void SolvesTrivialExample()
        {
            var rbm = new RBM(5, 2, 0.01, 0.1);
            double[][] examples = new double[2][];
            examples[0] = new double[]{1, 0, 0, 0, 0};
            examples[1] = new double[]{0, 0, 0, 0, 1};
            rbm.Train(examples, 100000);

            var activations = rbm.ComputeActivations(examples[0]);
            var reconstruction = rbm.Reconstruct(activations);
            Assert.IsTrue(reconstruction[0] > 0.9 && reconstruction[0] < 1.1);
            Assert.IsTrue(reconstruction[1] > -0.1 && reconstruction[1] < 0.1);
            Assert.IsTrue(reconstruction[2] > -0.1 && reconstruction[2] < 0.1);
            Assert.IsTrue(reconstruction[3] > -0.1 && reconstruction[3] < 0.1);
            Assert.IsTrue(reconstruction[4] > -0.1 && reconstruction[4] < 0.1);
        }
    }
}
