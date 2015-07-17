using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNNClassifier
{
    /// <summary>
    /// Single-layer Continuous Restricted Boltzmann Machine implementation
    /// Accepts fixed-length vectors of generic type
    /// </summary>
    [Serializable]
    public class RBM
    {
        public double[][] Weights;
        public double L;
        public double N;

        public double[][] TrainingSet;
        public int NumberOfCycles;

        [NonSerialized] private Thread _trainingThread = null;
        [NonSerialized] private bool stopTraining = false;

        private Random r;
        /// <summary>
        /// Single-layer Continuous Restricted Boltzmann Machine
        /// </summary>
        /// <param name="dataLength">Input vector size</param>
        /// <param name="numHiddenUnits">Hidden binary units</param>
        /// <param name="learningRate">Weight update multipler</param>
        /// <param name="noiseMagnitude">Noise magnitude in reconstruction</param>
        public RBM(int dataLength, int numHiddenUnits, double learningRate, double noiseMagnitude)
        {
            r = new Random();
            L = learningRate;
            N = noiseMagnitude;
            InitWeights(dataLength, numHiddenUnits);
            
        }

        public void Train(double[][] trainingSet, int numberOfCycles)
        {
            for (var i = 0; i < numberOfCycles; i++)
            {
                var index = i%trainingSet.Length;
                var input = trainingSet[index];
                UpdateWeights(input);
            }
        }

        public void TrainWorker()
        {
            var i = 0;
            while(true)
            {
                var index = (i++) % TrainingSet.Length;
                var input = TrainingSet[index];
                UpdateWeights(input);

                if(stopTraining)
                    _trainingThread.Abort();
            }
        }

        public void TrainMultithreaded(double[][] trainingSet, int numberOfCycles)
        {
            TrainingSet = trainingSet;
            NumberOfCycles = numberOfCycles;

            stopTraining = false;
            _trainingThread = new Thread(new ThreadStart(TrainWorker));
            _trainingThread.Start();
        }

        public void StopTrainMultithreaded()
        {
            stopTraining = true;
        }

        public void InitWeights(int dataLength, int numHiddenUnits)
        {
            Weights = new double[numHiddenUnits][];
            for (var i = 0; i < numHiddenUnits; i++)
            {
                Weights[i] = new double[dataLength];
                for (var j = 0; j < dataLength; j++)
                {
                    Weights[i][j] = r.NextDouble()/10;
                }
            }
        }

        private double ActivationEnergy(double[] input, int hiddenUnitNumber)
        {
            double sum = 0;
            for (var i = 0; i < Weights[hiddenUnitNumber].Length; i++)
            {
                sum += input[i]*Weights[hiddenUnitNumber][i];
            }

            return sum;
        }

        private double Sigma(double activationEnergy)
        {
            var sigma = 1/(1 + Math.Exp(-activationEnergy));
            return sigma;
        }

        public bool[] ComputeActivations(double[] input)
        {
            var activations = new bool[Weights.Length];
            for (var i = 0; i < Weights.Length; i++)
            {
                var p = Sigma(ActivationEnergy(input, i));
                activations[i] = (r.NextDouble() < p);
                activations[i] = (r.NextDouble() > 0.5) && activations[i];
            }
            return activations;
        }

        public double[] ComputeActivationsExact(double[] input)
        {
            var activations = new double[Weights.Length];
            for (var i = 0; i < Weights.Length; i++)
            {
                var p = Sigma(ActivationEnergy(input, i));
                activations[i] = p;
            }
            return activations;
        }

        private double[][] ComputeAgreement(double[] input, bool[] activations)
        {
            var agreement = new double[activations.Length][];
            for (var i = 0; i < activations.Length; i++)
            {
                var a = new double[input.Length];
                for (var j = 0; j < input.Length; j++)
                {
                    a[j] = activations[i] ? input[j] : 0;
                }
                agreement[i] = a;
            }
            
            return agreement;
        }

        public double[] ReconstructExact(double[] activations)
        {
            var reconstruction = new double[Weights[0].Length];
            for (var i = 0; i < reconstruction.Length; i++)
            {
                var sum = activations.Select((t, j) => t*Weights[j][i] ).Sum();
                var p = Sigma(sum);
                reconstruction[i] = p;
            }
            return reconstruction;
        }

        public double[] Reconstruct(bool[] activations)
        {
            var reconstruction = new double[Weights[0].Length];
            for (var i = 0; i < reconstruction.Length; i++)
            {
                var sum = activations.Select((t, j) => t ? Weights[j][i] : 0).Sum();
                var p = Sigma(sum);
                reconstruction[i] = p + N * (r.NextDouble() - 0.5);
            }
            return reconstruction;
        }

        private void UpdateWeights(double[] input)
        {
            var activations = ComputeActivations(input);
            var ePositive = ComputeAgreement(input, activations);
            var reconstruction = Reconstruct(activations);
            var ractivations = ComputeActivations(reconstruction);
            var eNegative = ComputeAgreement(reconstruction, ractivations);

            for(var i=0; i<Weights.Length; i++)
                for (var j = 0; j < Weights[i].Length; j++)
                {
                    Weights[i][j] = Weights[i][j] + L*(ePositive[i][j] - eNegative[i][j]);
                }

        }

    }
}
