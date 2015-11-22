﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MathNet.Numerics.Distributions;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Threading.Tasks;

namespace VTC.Kernel
{

    public class MoGBackground
    {
        public MixtureModel[,] mmImage; //2D array of mixture models
        public Image<Bgr, float> BackgroundUpdateMoG;
        private Mutex _updateMutex = new Mutex();

        public MoGBackground(int Width, int Height)
        {
            BackgroundUpdateMoG = new Image<Bgr, float>(Width, Height);
            mmImage = new MixtureModel[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    mmImage[i, j] = new MixtureModel();
        }

        public void TryUpdatingBackgroundAsync(Image<Bgr, Byte> frame)
        {
            Task.Factory.StartNew(() => UpdateBackgroundMoGIncremental(frame));
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
    }

    public class MixtureModel
    {

        private int[][] _samples; // _numSamples X _numDimensions
        public double[][] Assignments;
        private int _numSamples;

        private int _numDimensions;
        private int NumComponents;
        private int NumIterations;
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

        public MixtureModel()
        {            
            InitializeDistributionParameters();
        }

        private void InitializeDistributionParameters()
        {
            int seed = (int) DateTime.Now.Ticks%int.MaxValue;
            Random rnd = new Random(seed);
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

            //ReorderByLikelihood();
        }

        //Note: this function is not adapted to handle cases other than dimensionality of 3 with 2 elements.
        private void ReorderByLikelihood()
        {
            //Ensure that the Gaussians are returned in order of weight/ (total variance)
            double sumOfVariances0 = Variances[0][0] + Variances[0][1] + Variances[0][2];
            double sumOfVariances1 = Variances[1][0] + Variances[1][1] + Variances[1][2];

            double likelihood0 = Weights[0]/sumOfVariances0;
            double likelihood1 = Weights[1]/sumOfVariances1;
            if (likelihood1 > likelihood0)
            {
                double[] tempVariance = new double[3];
                double[] tempMean = new double[3];

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
                        likelihoods[j] = EvaluateProbability(j, k);
                            // Likelihood of sample k being produced by component j

                    int argmax = 0;
                    for (int j = 0; j < NumComponents; j++)
                        if (likelihoods[j] > likelihoods[argmax])
                            argmax = j;

                    double totalLikelihood = likelihoods.Aggregate((sum, next) => sum + next);
                    double[] probabilities = likelihoods.Select(l => l/totalLikelihood).ToArray();
                    for (int j = 0; j < NumComponents; j++)
                    {
                        Assignments[j][k] = probabilities[j];
                        
                        if (Double.IsNaN(Assignments[j][k]))
                        throw new Exception("Value is NaN 4");

                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }
        }



        // Likelihood of sample k being produced by component j
        private double EvaluateProbability(int j, int k)
        {
            double probability = 1;
            try
            {
                for (int i = 0; i < _numDimensions; i++)
                {
                    var gaussianComponent = new Normal(Means[j][i], Variances[j][i]);
                    double likelihood = gaussianComponent.Density(_samples[k][i]);
                    if (Double.IsNaN(likelihood)) //When variance is 0, Normal returns NaN for probability
                        likelihood = 0;

                    probability = probability*likelihood;
                }
                //probability = Weights[j]*probability;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }
            
            return probability;
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
                //probability = Weights[j]*probability;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.TargetSite + " " + e.StackTrace);
            }

            return probability;
        }
    }
}
