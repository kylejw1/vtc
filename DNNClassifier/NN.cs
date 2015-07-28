using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNClassifier
{
    public class NN
    {
        public double[][] Weights;
        public double[][] Inputs;
        public double[][] Targets;

        public double L;

        public NN(int inputLength, int outputLength, double learning_rate)
        {
            L = learning_rate;
            InitializeWeights(inputLength+1, outputLength);
        }

        public double[] Evaluate(double[] input)
        {
            double[] output = new double[Weights.Length];
            for (var i = 0; i < output.Length;i++)
            {
                var input_biased = AppendBias(input);
                var neuron_input = Hadamard(input_biased, Weights[i]);
                output[i] = Logistic(neuron_input);
            }
                
            return output;
        }

        private double Hadamard(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new Exception("Cannot Hadamard-product inequal length vectors");

            double result = 0;
            for (var i = 0; i < a.Length; i++)
                result += a[i] * b[i];

            return result;
        }

        private double[] AppendBias(double[] input)
        {
            double[] input_with_bias = new double[input.Length + 1];
            input_with_bias[0] = 1;
            for (var i = 1; i <= input.Length; i++)
                input_with_bias[i] = input[i - 1];

            return input_with_bias;
        }

        private double Logistic(double input)
        {
            return 1 / (1 + Math.Exp(-input));
        }

        public void Train(double[][] inputs, double[][] targets, int training_iterations)
        {
            for(var i=0;i<training_iterations; i++)
            {
                double[] output = Evaluate(inputs[i%inputs.Length]);
                double[][] weight_gradients = WeightGradients(inputs[i%inputs.Length], output, targets[i%inputs.Length] );
                UpdateWeights(weight_gradients);
            }
        }

        void UpdateWeights(double[][] weight_gradients)
        {
            for(var i=0;i<Weights.Length;i++)
                for (var j = 0; j < Weights[i].Length; j++)
                    Weights[i][j] -= L * weight_gradients[i][j];
        }

        double[][] WeightGradients(double[] input, double[] output, double[] target)
        {
            double[][] weight_gradients = new double[Weights.Length][];
            double[] error = Error(output, target);
            double[] input_biased = AppendBias(input);
            for(var i=0;i<Weights.Length;i++)
            {
                double[] weight_gradient = new double[input.Length+1];
                for (var j = 0; j < input.Length+1; j++)
                    weight_gradient[j] = error[i] * input_biased[j];

                weight_gradients[i] = weight_gradient;
            }

            return weight_gradients;
        }

        double[] Error(double[] output, double[] target)
        {
            double[] error = new double[output.Length];
            for (var j = 0; j < output.Length; j++)
                error[j] = output[j] - target[j];

            return error;
        }

        public void InitializeWeights(int nInputs, int nOutputs)
        {
            Random r = new Random();
            Weights = new double[nOutputs][];
            for(var i=0;i<nOutputs;i++)
            {
                Weights[i] = new double[nInputs];
                for (var j = 0; j < nInputs; j++)
                    Weights[i][j] = r.NextDouble();
            }
        }

        public double AverageError(double[][] inputs, double[][] targets)
        {
            double errorSum = 0;
            for (var i = 0; i < inputs.Length; i++)
            {
                double[] output = Evaluate(inputs[i]);
                double[] error = Error(output, targets[i]);
                errorSum += error.Sum();
            }

            return errorSum/inputs.Length;
        }

    }
}
