using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNNClassifier
{
    /// <summary>
    /// Single-layer Continuous Restricted Boltzmann Machine implementation
    /// Accepts fixed-length vectors of double
    /// </summary>
    [Serializable]
    public class RBM
    {
        public double[][] Weights;
        private readonly double _l;
        private readonly double _n;
        public int TrainingCycles;
        private double[][] _trainingSet;
        private readonly Random _r;

        [NonSerialized] private Thread _trainingThread = null;
        [NonSerialized] private bool _stopTraining = false;

        /// <summary>
        /// Single-layer Continuous Restricted Boltzmann Machine
        /// </summary>
        /// <param name="dataLength">Input vector size</param>
        /// <param name="numHiddenUnits">Hidden binary units</param>
        /// <param name="learningRate">Weight update multipler</param>
        /// <param name="noiseMagnitude">Noise magnitude in reconstruction</param>
        public RBM(int dataLength, int numHiddenUnits, double learningRate, double noiseMagnitude)
        {
            _r = new Random();
            _l = learningRate;
            _n = noiseMagnitude;
            TrainingCycles = 0;
            InitWeights(dataLength, numHiddenUnits);
        }

        /// <summary>
        /// Train RBM weights for a fixed number of iterations, single-threaded.
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="numberOfCycles"></param>
        public void Train(double[][] trainingSet, int numberOfCycles)
        {
            for (var i = 0; i < numberOfCycles; i++)
            {
                var index = i%trainingSet.Length;
                var input = trainingSet[index];
                UpdateWeights(input);
                TrainingCycles++;
            }
        }

        /// <summary>
        /// Worker thread for training RBM weights.
        /// </summary>
        private void TrainWorker()
        {
            var i = 0;
            while(true)
            {
                var index = (i++) % _trainingSet.Length;
                var input = _trainingSet[index];
                UpdateWeights(input);
                TrainingCycles++;

                if (!_stopTraining) continue;
                _trainingThread.Abort();
                return;
            }
        }

        /// <summary>
        /// Launch worker thread.
        /// </summary>
        /// <param name="trainingSet"></param>
        public void TrainMultithreaded(double[][] trainingSet)
        {
            _trainingSet = trainingSet;
            _stopTraining = false;
            _trainingThread = new Thread(TrainWorker);
            _trainingThread.Start();
        }

        /// <summary>
        /// Stop worker thread.
        /// </summary>
        public void StopTrainMultithreaded()
        {
            _stopTraining = true;
        }

        /// <summary>
        /// Randomly initialize RBM weights.
        /// </summary>
        /// <param name="dataLength">Number of input units per hidden unit.</param>
        /// <param name="numHiddenUnits">Number of hidden units.</param>
        private void InitWeights(int dataLength, int numHiddenUnits)
        {
            Weights = new double[numHiddenUnits][];
            for (var i = 0; i < numHiddenUnits; i++)
            {
                Weights[i] = new double[dataLength];
                for (var j = 0; j < dataLength; j++)
                {
                    Weights[i][j] = _r.NextDouble()/10;
                }
            }
        }

        /// <summary>
        /// Calculate activation energy for a particular hidden unit, given an input vector.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hiddenUnitNumber"></param>
        /// <returns></returns>
        private double ActivationEnergy(double[] input, int hiddenUnitNumber)
        {
            double sum = 0;
            for (var i = 0; i < Weights[hiddenUnitNumber].Length; i++)
            {
                sum += input[i]*Weights[hiddenUnitNumber][i];
            }

            return sum;
        }

        /// <summary>
        /// Calculate unit activation using sigmoid logistic function.
        /// </summary>
        /// <param name="activationEnergy"></param>
        /// <returns></returns>
        private static double Sigma(double activationEnergy)
        {
            var sigma = 1/(1 + Math.Exp(-activationEnergy));
            return sigma;
        }

        /// <summary>
        /// Compute boolean hidden-unit activation given an input vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool[] ComputeActivations(double[] input)
        {
            var activations = new bool[Weights.Length];
            for (var i = 0; i < Weights.Length; i++)
            {
                var p = Sigma(ActivationEnergy(input, i));
                activations[i] = (_r.NextDouble() < p);
                activations[i] = (_r.NextDouble() > 0.5) && activations[i];
            }
            return activations;
        }

        /// <summary>
        /// Compute real-valued hidden unit activation given an input vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Compute agreement between a set of input vectors and hidden unit activations.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="activations"></param>
        /// <returns></returns>
        private static double[][] ComputeAgreement(double[] input, bool[] activations)
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

        /// <summary>
        /// Reconstruct input given real-valued hidden unit activations. 
        /// </summary>
        /// <param name="activations"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Reconstruct input given boolean-valued hidden unit activations.
        /// </summary>
        /// <param name="activations"></param>
        /// <returns></returns>
        public double[] Reconstruct(bool[] activations)
        {
            var reconstruction = new double[Weights[0].Length];
            for (var i = 0; i < reconstruction.Length; i++)
            {
                var sum = activations.Select((t, j) => t ? Weights[j][i] : 0).Sum();
                var p = Sigma(sum);
                reconstruction[i] = p + _n * (_r.NextDouble() - 0.5);
            }
            return reconstruction;
        }

        /// <summary>
        /// Perform a single weight-update cycle using an input vector.
        /// </summary>
        /// <param name="input"></param>
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
                    Weights[i][j] = Weights[i][j] + _l*(ePositive[i][j] - eNegative[i][j]);
                }

        }

        /// <summary>
        /// Serialize parameters to a file on disk.
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
        /// Deserialize parameters from disk.
        /// </summary>
        /// <param name="path">Location to read serialized object.</param>
        /// <returns></returns>
        public static RBM ImportWeights(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = (RBM)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        /// <summary>
        /// Render weights as 30x30 pixel color bitmaps and write to disk.
        /// </summary>
        /// <param name="exportPath"></param>
        public void ExportWeightVisualizations(string exportPath)
        {
                var dc = new RBMDataConverter();

                var maxWeight = 0.0;
                var minWeight = 0.0;
                const double maxAfter = 255.0;
                const double minAfter = 0.0;

                for (var i = 1; i < Weights.Length; i++)
                    for (int j = 1; j < Weights[i].Length; j++)
                        if (Weights[i][j] > maxWeight)
                            maxWeight = Weights[i][j];

                for (var i = 1; i < Weights.Length; i++)
                    for (var j = 1; j < Weights[i].Length; j++)
                        if (Weights[i][j] < minWeight)
                            minWeight = Weights[i][j];

                var transformedWeights = new double[Weights.Length][];
                for (var i = 0; i < Weights.Length; i++)
                {
                    var newWeightsArray = new double[Weights[0].Length];
                    for (int j = 0; j < Weights[i].Length; j++)
                        newWeightsArray[j] = (Weights[i][j] - minWeight) * (maxAfter - minAfter) / (maxWeight - minWeight);

                    transformedWeights[i] = newWeightsArray;
                }

                for (var i = 0; i < Weights.Length; i++)
                {
                    dc.SaveRawDataToImage(transformedWeights[i], exportPath + "\\" + i + ".bmp", 30, 30);
                }
        }

        /// <summary>
        /// Export reconstruction of single image, return activations. 
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="exportPath"></param>
        /// <returns></returns>
        public double[] ExportSingleReconstruction(string inputPath, string exportPath)
        {
            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromSingleImage(inputPath);
            var activations = ComputeActivationsExact(trainingData[0]);
            var reconstruction = ReconstructExact(activations);
            dc.SaveDataToImage(reconstruction, exportPath, 30, 30);
            return activations;
        }

    }
}
