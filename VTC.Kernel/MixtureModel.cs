using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MathNet.Numerics.Distributions;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace VTC.Kernel
{

    public class MoGBackground
    {
        public MixtureModel[,] mmImage; //2D array of mixture models
        public Image<Bgr, float> BackgroundUpdateMoG;
        private Mutex _updateMutex = new Mutex();
        List<Image<Bgr, Byte>> inputImagesList;
        private const int numberOfSamples = 50;
        private int width, height;

        public MoGBackground(int Width, int Height)
        {
            width = Width;
            height = Height;
            BackgroundUpdateMoG = new Image<Bgr, float>(Width, Height);
            inputImagesList = new List<Image<Bgr, byte>>();
            mmImage = new MixtureModel[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    mmImage[i, j] = new MixtureModel();
        }

        public void TryUpdatingBackgroundAsync(Image<Bgr, Byte> frame)
        {
            Task.Factory.StartNew(() => UpdateBackgroundMoGBatch(frame));
        }

        public void UpdateBackgroundMoGIncremental(Image<Bgr, Byte> frame)
        {
            // If we're already busy updating background, just abort
            if (!_updateMutex.WaitOne(0))
            {
                return;
            }

            try
            {
                Image<Bgr, float> newImageSample = frame.Convert<Bgr, float>();
                for (int i = 0; i < newImageSample.Width; i++)
                    for (int j = 0; j < newImageSample.Height; j++)
                    {
                        var samplePoint = new int[] { frame.Data[j, i, 0], frame.Data[j, i, 1], frame.Data[j, i, 2] };
                        mmImage[i, j].TrainIncremental(samplePoint);

                        BackgroundUpdateMoG.Data[j, i, 0] = (float)mmImage[i, j].Means[0][0];
                        BackgroundUpdateMoG.Data[j, i, 1] = (float)mmImage[i, j].Means[0][1];
                        BackgroundUpdateMoG.Data[j, i, 2] = (float)mmImage[i, j].Means[0][2];
                    }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
            
        }

        public void UpdateBackgroundMoGBatch(Image<Bgr, Byte> frame)
        {
            // If we're already busy updating background, just abort
            if (!_updateMutex.WaitOne(0))
            {
                return;
            }

            try
            {
                inputImagesList.Insert(0, frame);
                while (inputImagesList.Count > numberOfSamples)
                    inputImagesList.Remove(inputImagesList.LastOrDefault());

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {   
                        var samplePoints = new int[inputImagesList.Count][];
                        for(int k = 0; k < inputImagesList.Count; k++)
                            samplePoints[k] = new int[] { inputImagesList.ElementAt(k).Data[j, i, 0], inputImagesList.ElementAt(k).Data[j, i, 1], inputImagesList.ElementAt(k).Data[j, i, 2] };

                        mmImage[i, j]._samples = samplePoints;
                        mmImage[i, j].NumComponents = 2;
                        mmImage[i, j].NumIterations = 3;
                        mmImage[i, j].Initialize();
                        mmImage[i, j].Train();
                        mmImage[i, j].ReorderByLikelihood();

                        BackgroundUpdateMoG.Data[j, i, 0] = (float)mmImage[i, j].Means[0][0];
                        BackgroundUpdateMoG.Data[j, i, 1] = (float)mmImage[i, j].Means[0][1];
                        BackgroundUpdateMoG.Data[j, i, 2] = (float)mmImage[i, j].Means[0][2];
                    }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }

        }
    }

    public class MixtureModel
    {

        public int[][] _samples; // _numSamples X _numDimensions
        public double[][] Assignments;
        private int _numSamples;

        private int _numDimensions;
        public int NumComponents;
        public int NumIterations;
        private const double Alpha = 0.3;

        private const double _varianceMax = 50;
        private const double _varianceMin = 0.01;

        public double[][] Means;
        public double[][] Variances;
        public double[] Weights;

        public MixtureModel(int[][] samplesIn, int numComponents, int numIterations)
        {
            NumComponents = numComponents;
            NumIterations = numIterations;
            
            Means = new double[NumComponents][];
            Variances = new double[NumComponents][];
            Weights = new double[NumComponents];

            _samples = samplesIn;
            _numSamples = _samples.Length;
            _numDimensions = _samples[0].Length;
            Assignments = new double[NumComponents][];
            for (int i = 0; i < NumComponents; i++)
                Assignments[i] = new double[_numSamples];

            InitializeDistributionParameters();
        }

        public void Initialize()
        {
            _numSamples = _samples.Length;
            _numDimensions = _samples[0].Length;
            Assignments = new double[NumComponents][];
            for (int i = 0; i < NumComponents; i++)
                Assignments[i] = new double[_numSamples];

            Means = new double[NumComponents][];
            Variances = new double[NumComponents][];
            Weights = new double[NumComponents];

            InitializeDistributionParameters();
        }

        /// <summary>
        /// Default constructor for 2-component, 3-dimensional (2x RGB) Gaussians
        /// </summary>
        public MixtureModel()
        {
            InitializeDefaultDistributionParameters();
        }

        private void InitializeDefaultDistributionParameters()
        {
            _numDimensions = 3;
            NumComponents = 2;
           InitializeDistributionParameters();
        }

        private void InitializeDistributionParameters()
        {
            int seed = (int) DateTime.Now.Ticks%int.MaxValue;
            Random rnd = new Random(seed);
            Means = new double[NumComponents][];
            Variances = new double[NumComponents][];
            Weights = new double[NumComponents];
            for (int i = 0; i < NumComponents; i++)
            {
                Means[i] = new double[_numDimensions];
                Variances[i] = new double[_numDimensions];

                int val = rnd.Next(0, 255);
                for (int j = 0; j < _numDimensions; j++)
                {
                    Means[i][j] = val;
                    Variances[i][j] = _varianceMax;
                }
                Weights[i] = (double) 1/NumComponents;
            }
        }

        public void Train()
        {
            for (int i = 0; i < NumIterations; i++)
            {
                CalculateAssignments();
                UpdateParameters();
            }
        }

        //Note: this function is not adapted to handle cases other than dimensionality of 3 with 2 elements.
        public void ReorderByLikelihood()
        {
            //Ensure that the Gaussians are returned in order of weight/ (total variance)
            double sumOfVariances0 = Variances[0][0] + Variances[0][1] + Variances[0][2];
            double sumOfVariances1 = Variances[1][0] + Variances[1][1] + Variances[1][2];

            double likelihood0 = Weights[0] / sumOfVariances0;
            double likelihood1 = Weights[1] / sumOfVariances1;

            double assignments0 = 0;
            for (int i = 0; i < _numSamples; i++)
                assignments0 += Assignments[0][i];

            double assignments1 = 0;
            for (int i = 0; i < _numSamples; i++)
                assignments1 += Assignments[1][i];

            if ((likelihood1 > likelihood0 && assignments1 > 5.0) || assignments0 < 5.0)
            {
                double[] tempVariance = new double[3];
                double[] tempMean = new double[3];
                double[] tempWeight = new double[2];

                tempVariance[0] = Variances[0][0];
                tempVariance[1] = Variances[0][1];
                tempVariance[2] = Variances[0][2];

                Variances[0][0] = Variances[1][0];
                Variances[0][1] = Variances[1][1];
                Variances[0][2] = Variances[1][2];

                Variances[1][0] = tempVariance[0];
                Variances[1][1] = tempVariance[1];
                Variances[1][2] = tempVariance[2];

                tempMean[0] = Means[0][0];
                tempMean[1] = Means[0][1];
                tempMean[2] = Means[0][2];

                Means[0][0] = Means[1][0];
                Means[0][1] = Means[1][1];
                Means[0][2] = Means[1][2];

                Means[1][0] = tempMean[0];
                Means[1][1] = tempMean[1];
                Means[1][2] = tempMean[2];

                tempWeight[0] = Weights[0];
                tempWeight[1] = Weights[1];

                Weights[0] = tempWeight[1];
                Weights[1] = tempWeight[0];
            }
        }


        // Online Mixture-of-Gaussians incremental calculation, 
        // taken from Stauffer and Grimson: "Adaptive background mixture models for real-time tracking"
        public void TrainIncremental(int[] rgb)
        {
            // Assign sample to Gaussian
            //Calculate probability of assignment to each Gaussian
            //Search array for max
            double[] assignmentProbabilities = new double[NumComponents];
            double assignmentMax = 0;
            int mostLikelyGaussian = 0;
            for (int i = 0; i < NumComponents; i++)
            {
                assignmentProbabilities[i] = EvaluateProbability(i, rgb);
                if (assignmentProbabilities[i] > assignmentMax)
                {
                    assignmentMax = assignmentProbabilities[i];
                    mostLikelyGaussian = i;
                }             
            }

            for (int i = 0; i < _numDimensions; i++)
            {
                // *********************  Update Gaussians ***************************//
                //Update means
                // u_next = (1-alpha)u_prev + alpha(sample)
                Means[mostLikelyGaussian][i] = (1 - Alpha) * (Means[mostLikelyGaussian][i]) + Alpha * rgb[i];

                //Update variances
                // ss_next = (1-alpha)ss + alpha(sample-u)(sample-u)
                Variances[mostLikelyGaussian][i] = (1 - Alpha) * (Variances[mostLikelyGaussian][i]) +
                                                   Alpha * Math.Pow((rgb[i] - Means[mostLikelyGaussian][i]), 2);
                Variances[mostLikelyGaussian][i] = (Variances[mostLikelyGaussian][i] < _varianceMin) ? _varianceMin : Variances[mostLikelyGaussian][i];                
            }
            
            //Update weights
            //  w_next = (1-alpha)w_prev + alpha(M)
            // *M is 1 for the matching Gaussian and 0 otherwise
            for (int i = 0; i < NumComponents; i++)
            {
                if (i == mostLikelyGaussian)
                {
                    Weights[i] = (1 - Alpha)*Weights[i] + Alpha;
                }
                else
                {
                    Weights[i] = (1 - Alpha) * Weights[i];
                }
            }

            ReorderByLikelihood();

        }

        private void UpdateParameters()
        {
            try
            {
                for (int j = 0; j < NumComponents; j++)
                {
                    // Calculate mean
                    double assignedToThisComponent = 0;
                    for (int i = 0; i < _numSamples; i++)
                        assignedToThisComponent += Assignments[j][i];

                    double[] sums = new double[_numDimensions];
                    for (int k = 0; k < _numSamples; k++)
                        for (int m = 0; m < _numDimensions; m++)
                            sums[m] = sums[m] + Assignments[j][k]*_samples[k][m];

                    for (int m = 0; m < _numDimensions; m++)
                    {
                        Means[j][m] = (double)sums[m] / (assignedToThisComponent);
                        if (Double.IsNaN(Means[j][m]))
                            throw new Exception("Value is NaN 1");    
                    }
                    
                    // Calculate variance
                    double[] varianceSums = new double[_numDimensions];
                    for (int k = 0; k < _numSamples; k++)
                        for (int m = 0; m < _numDimensions; m++)
                            varianceSums[m] = varianceSums[m] +
                                              Assignments[j][k]*Math.Pow((_samples[k][m] - Means[j][m]), 2);

                    for (int m = 0; m < _numDimensions; m++)
                    {
                        Variances[j][m] = Math.Sqrt(varianceSums[m] / (assignedToThisComponent));
                        if (Double.IsNaN(Variances[j][m]))
                            throw new Exception("Value is NaN 2");

                        if (Variances[j][m] > _varianceMax)
                            Variances[j][m] = _varianceMax;
                        if (Variances[j][m] < _varianceMin)
                            Variances[j][m] = _varianceMin;    
                    }

                    double thisComponentWeight = 0;
                    for (int i = 0; i < _numSamples; i++)
                        thisComponentWeight += Assignments[j][i] / _numSamples;

                    Weights[j] = thisComponentWeight;
                    if (Weights[j] == 0.0) // Prevent divide-by-zero errors
                    {
                        Weights[j] = 0.01;
                        if (Double.IsNaN(Weights[j]))
                            throw new Exception("Value is NaN 3");
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }
            
        }

        private void CalculateAssignments()
        {
            try
            {
                for (int k = 0; k < _numSamples; k++)
                {
                    double[] likelihoods = new double[NumComponents];
                    for (int j = 0; j < NumComponents; j++)
                        likelihoods[j] = EvaluateLikelihood(j, k);

                    int argmax = 0;
                    for (int j = 0; j < NumComponents; j++)
                        if (likelihoods[j] > likelihoods[argmax])
                            argmax = j;

                    double totalLikelihood = likelihoods.Aggregate((sum, next) => sum + next);
                    double[] probabilities = likelihoods.Select(l => l/totalLikelihood).ToArray();
                    for (int j = 0; j < NumComponents; j++)
                        Assignments[j][k] = probabilities[j];
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }
        }


        /// <summary>
        /// Likelihood of sample k being produced by component j
        /// </summary>
        /// <param name="j">component</param>
        /// <param name="k">sample</param>
        /// <returns></returns>
        private double EvaluateLikelihood(int j, int k)
        {
            double likelihood = 1;
            try
            {
                for (int i = 0; i < _numDimensions; i++)
                    likelihood = likelihood * SingleGaussianLikelihood(j, i, _samples[k]);   
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }

            likelihood *= Weights[j];
            
            return likelihood;
        }

        /// <summary>
        /// Likelihood of a single-dimensional gaussian generating a sample point
        /// </summary>
        /// <param name="j">component</param>
        /// <param name="i">dimension</param>
        /// <param name="sample">sample vector</param>
        /// <returns></returns>
        private double SingleGaussianLikelihood(int j, int i, int[] sample)
        {
            if (Means?[j] == null || Variances?[j] == null)
                return 0;

            var mean = Means[j][i];
            var variance = Variances[j][i];
            Normal gaussianComponent = new Normal(mean, variance);
            double likelihood = gaussianComponent.Density(sample[i]);
            if (Double.IsNaN(likelihood)) //When variance is 0, Normal returns NaN for probability
                likelihood = 0;

            return likelihood;
        }


        /// <summary>
        /// Likelihood of a multidimensional sample point having been generated by a single, multidimensional Gaussian
        /// </summary>
        /// <param name="j"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        private double MultidimensionalGaussianLikelihood(int j, int[] sample)
        {
            if (Means == null || Variances == null)
                return 0;

            double likelihood = 1;
            for (int i = 0; i < _numDimensions; i++)
                likelihood = likelihood * SingleGaussianLikelihood(j, i, sample);
            
            return likelihood;
        }

        public bool IsForegroundSample(int[] sample)
        {
            double p0 = MultidimensionalGaussianLikelihood(0, sample);
            double p1 = MultidimensionalGaussianLikelihood(1, sample);
            //if (p1 > p0)
            //    return true;

            if (p1/p0 > 10)
                return true;

            return false;
        }


        // Likelihood of sample k being produced by component j
        private double EvaluateProbability(int j, int[] rgb)
        {
            double probability = 1;
            try
            {
                for (int i = 0; i < _numDimensions; i++)
                {
                    var gaussianComponent = new Normal(Means[j][i], Variances[j][i]);
                    double likelihood = gaussianComponent.Density(rgb[i]);
                    if (Double.IsNaN(likelihood)) //When variance is 0, Normal returns NaN for probability
                        likelihood = 0;

                    probability = probability * likelihood;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }

            return probability;
        }
    }
}
