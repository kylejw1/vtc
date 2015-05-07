using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace VTC.Kernel
{
    public class MixtureModel
    {

        private int[][] _samples; // _numSamples X _numDimensions
        public double[][] Assignments;
        private int _numSamples;
        private int _numDimensions;
        private const int NumComponents = 2;
        private const int NumIterations = 2;

        private const double _varianceMax = 50;
        private const double _varianceMin = 1;

        public double[][] Means = new double[NumComponents][];
        public double[][] Variances = new double[NumComponents][];
        public double[] Weights = new double[NumComponents];

        public MixtureModel(int[][] samplesIn)
        {
            _samples = samplesIn;
            _numSamples = _samples.Length;
            _numDimensions = _samples[0].Length;
            Assignments = new double[NumComponents][];

            InitializeDistributionParameters();
        }

        private void InitializeDistributionParameters()
        {
            Random rnd = new Random();
            for (int i = 0; i < NumComponents; i++)
            {
                Means[0] = new double[_numDimensions];
                Means[1] = new double[_numDimensions];
                //Means[0][0] = rnd.Next(0, 255); // Assuming R,G,B pixel color input data
                //Means[0][1] = rnd.Next(0, 255);
                //Means[0][2] = rnd.Next(0, 255);
                //Means[1][0] = rnd.Next(0, 255);
                //Means[1][1] = rnd.Next(0, 255);
                //Means[1][2] = rnd.Next(0, 255);
                Means[0][0] = 0; 
                Means[0][1] = 0;
                Means[0][2] = 0;
                Means[1][0] = 255;
                Means[1][1] = 255;
                Means[1][2] = 255;

                Variances[0] = new double[_numDimensions];
                Variances[1] = new double[_numDimensions];
                //Variances[0][0] = rnd.Next(5, 100);
                //Variances[0][1] = rnd.Next(5, 100);
                //Variances[0][2] = rnd.Next(5, 100);
                //Variances[1][0] = rnd.Next(5, 100);
                //Variances[1][1] = rnd.Next(5, 100);
                //Variances[1][2] = rnd.Next(5, 100);
                Variances[0][0] = _varianceMax;
                Variances[0][1] = _varianceMax;
                Variances[0][2] = _varianceMax;
                Variances[1][0] = _varianceMax;
                Variances[1][1] = _varianceMax;
                Variances[1][2] = _varianceMax;
                Weights[i] = (double) 1/NumComponents;

                Assignments[i] = new double[_numSamples];
            }

            // Make sure each component is initialized with some members so that updates can be calculated
            for(int j=0; j<NumComponents; j++)
            for (int i = 0; i < _numSamples; i++)
                Assignments[j][i] = (double) 1/NumComponents;
        }

        public void Train()
        {
            for (int i = 0; i < NumIterations; i++)
            {
                // Calculate most likely assignments
                CalculateAssignments();
                // Update distribution parameters
                UpdateParameters();

                PrintParameters();
            }

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
                            throw new Exception("Value is NaN");    
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
                            throw new Exception("Value is NaN");

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
                            throw new Exception("Value is NaN");
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Got exception");
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
                        throw new Exception("Value is NaN");

                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Got exception");
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
                System.Diagnostics.Debug.WriteLine("Got exception");
            }
            
            return probability;
        }

        private void PrintParameters()
        {
            for (int i = 0; i < NumComponents;i ++)
            {
                System.Diagnostics.Debug.WriteLine("Means[0]: ");
                System.Diagnostics.Debug.WriteLine(Means[0][0] + ", " + Means[0][1] + ", " + Means[0][2]);
                System.Diagnostics.Debug.WriteLine("Means[1]: ");
                System.Diagnostics.Debug.WriteLine(Means[1][0] + ", " + Means[1][1] + ", " + Means[1][2]);

                System.Diagnostics.Debug.WriteLine("Variances[0]: ");
                System.Diagnostics.Debug.WriteLine(Variances[0][0] + ", " + Variances[0][1] + ", " + Variances[0][2]);
                System.Diagnostics.Debug.WriteLine("Variances[1]: ");
                System.Diagnostics.Debug.WriteLine(Variances[1][0] + ", " + Variances[1][1] + ", " + Variances[1][2]);

                //System.Diagnostics.Debug.WriteLine("Assignments[0]: "+string.Join(",", Assignments[0]));
                //System.Diagnostics.Debug.WriteLine("Assignments[1]: " + string.Join(",", Assignments[1]));

                System.Diagnostics.Debug.WriteLine("Weights: " + string.Join(",", Weights));
            }

        }
    }
}
