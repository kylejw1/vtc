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

        [TestMethod]
        public void NNSolvesTrivialExample()
        {
            var nn = new NN(1, 1, 0.1);

            double[][] inputs = new double[2][];
            inputs[0] = new double[] { 1 };
            inputs[1] = new double[] { 0 };

            double[][] targets = new double[2][];
            targets[0] = new double[] { 0 };
            targets[1] = new double[] { 1 };

            nn.Train(inputs, targets, 1000);

            var output0 = nn.Evaluate( new[] {0.0} );
            var output1 = nn.Evaluate( new[] {1.0} );


            Assert.IsTrue(output0[0] > 0.9 && output0[0] < 1.1);
            Assert.IsTrue(output1[0] < 0.1 && output1[0] > -0.1);
        }
    }
}
