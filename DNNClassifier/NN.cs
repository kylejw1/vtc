using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DNNClassifier
{
    /// <summary>
    /// Single-layer neural network implementation. Single row of N inputs, single row of M outputs. Backpropagations used for training.
    /// </summary>
    [Serializable]
    public class NN
    {
        private double[][] _weights;
        public double[][] Inputs;
        public double[][] Targets;
        public string[] Classes;
        public int TrainingCycles;
        private readonly double _l;

        public NN(int inputLength, int outputLength, double learningRate, string[] classes)
        {
            _l = learningRate;
            InitializeWeights(inputLength+1, outputLength);
            TrainingCycles = 0;
            Classes = classes;
        }

        /// <summary>
        /// Evaluate network output for a single given input vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Evaluate(double[] input)
        {
            var output = new double[_weights.Length];
            for (var i = 0; i < output.Length;i++)
            {
                var inputBiased = AppendBias(input);
                var neuronInput = Hadamard(inputBiased, _weights[i]);
                output[i] = Logistic(neuronInput);
            }
                
            return output;
        }

        /// <summary>
        /// Calculate Hadamard product between vectors (multiplication elementwise)
        /// </summary>
        /// <param name="a">Nx1-element array of doubles</param>
        /// <param name="b">Nx1-element array of doubles</param>
        /// <returns></returns>
        private static double Hadamard(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new Exception("Cannot Hadamard-product inequal length vectors");

            return a.Select((t, i) => t*b[i]).Sum();
        }

        /// <summary>
        /// Append bias input to an input vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static double[] AppendBias(double[] input)
        {
            var inputWithBias = new double[input.Length + 1];
            inputWithBias[0] = 1;
            for (var i = 1; i <= input.Length; i++)
                inputWithBias[i] = input[i - 1];

            return inputWithBias;
        }

        /// <summary>
        /// Calculate sigmoid logistic function of a real value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static double Logistic(double input)
        {
            return 1 / (1 + Math.Exp(-input));
        }

        /// <summary>
        /// Train a network for a given number of iterations, using a given input/output set.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="targets"></param>
        /// <param name="trainingIterations"></param>
        public void Train(double[][] inputs, double[][] targets, int trainingIterations)
        {
            for(var i=0;i<trainingIterations; i++)
            {
                var output = Evaluate(inputs[i%inputs.Length]);
                var weightGradients = WeightGradients(inputs[i%inputs.Length], output, targets[i%inputs.Length] );
                UpdateWeights(weightGradients);
                TrainingCycles++;
            }
        }

        /// <summary>
        /// Update weights as a function of weight gradients from a training cycle.
        /// </summary>
        /// <param name="weightGradients">Derivative of output error with respect to weights.</param>
        void UpdateWeights(double[][] weightGradients)
        {
            for(var i=0;i<_weights.Length;i++)
                for (var j = 0; j < _weights[i].Length; j++)
                    _weights[i][j] -= _l * weightGradients[i][j];
        }

        /// <summary>
        /// Calculate weight gradients from a single input/output pair
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double[][] WeightGradients(double[] input, double[] output, double[] target)
        {
            var weightGradients = new double[_weights.Length][];
            var error = Error(output, target);
            var inputBiased = AppendBias(input);
            for(var i=0;i<_weights.Length;i++)
            {
                var weightGradient = new double[input.Length+1];
                for (var j = 0; j < input.Length+1; j++)
                    weightGradient[j] = error[i] * inputBiased[j];

                weightGradients[i] = weightGradient;
            }

            return weightGradients;
        }

        /// <summary>
        /// Calculate error between network's output and the target vector.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        static double[] Error(double[] output, double[] target)
        {
            var error = new double[output.Length];
            for (var j = 0; j < output.Length; j++)
                error[j] = output[j] - target[j];

            return error;
        }

        /// <summary>
        /// Randomly initialize network weights. 
        /// </summary>
        /// <param name="nInputs"></param>
        /// <param name="nOutputs"></param>
        private void InitializeWeights(int nInputs, int nOutputs)
        {
            var r = new Random();
            _weights = new double[nOutputs][];
            for(var i=0;i<nOutputs;i++)
            {
                _weights[i] = new double[nInputs];
                for (var j = 0; j < nInputs; j++)
                    _weights[i][j] = r.NextDouble();
            }
        }

        /// <summary>
        /// Calculate average error on entire training set. Normally used for evaluation purposes (not training).
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double AverageError(double[][] inputs, double[][] targets)
        {
            var errorSum = inputs.Select(Evaluate).Select((output, i) => Error(output, targets[i])).Sum(error => error.Sum());
            return errorSum/inputs.Length;
        }

        /// <summary>
        /// Write parameters to file on disk.
        /// </summary>
        /// <param name="path">Location to write serialized object.</param>
        public void ExportWeights(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Load parameters from file on disk.
        /// </summary>
        /// <param name="path">Location to read serialized object.</param>
        /// <returns></returns>
        public static NN ImportWeights(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = (NN)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }


        public string ClassifyInput(double[] input)
        {
            double[] output = Evaluate(input);
            double maxClassifierOutput = output.Max();
            int maxIndex = output.ToList().IndexOf(maxClassifierOutput);
            return Classes[maxIndex];    
        }

    }
}
