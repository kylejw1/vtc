using System;
using System.Linq;
using System.Threading;
using MathNet.Numerics.Distributions;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VTC.Kernel
{
    /// <summary>
    /// 2D array of Gaussian mixture models, used to identify foreground pixels in video
    /// </summary>
    public class MoGBackground
    {
        /// <summary>
        /// 2D array of mixture models
        /// </summary>
        public readonly MixtureModel[,] MmImage;
        private int Width => MmImage.GetLength(0);
        private int Height => MmImage.GetLength(1);

        /// <summary>
        /// A mutex to track asynchronous background updates
        /// </summary>
        private readonly Mutex _updateMutex = new Mutex();

        public MoGBackground(int width, int height)
        {
            MmImage = new MixtureModel[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    MmImage[i, j] = new MixtureModel();
        }

        public void TryUpdatingAsync(Image<Bgr, Byte> frame)
        {
            Task.Factory.StartNew(() => Update(frame));
        }

        private void Update(Image<Bgr, Byte> frame)
        {
            // If we're already busy updating background, just abort
            if (!_updateMutex.WaitOne(0))
                return;
            
            try
            {
                var newImageSample = frame.Convert<Bgr, float>();
                for (var i = 0; i < newImageSample.Width; i++)
                    for (var j = 0; j < newImageSample.Height; j++)
                    {
                        var samplePoint = new int[] { frame.Data[j, i, 0], frame.Data[j, i, 1], frame.Data[j, i, 2] };
                        MmImage[i, j].TrainIncremental(samplePoint);
                    }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
            
        }

        /// <summary>
        /// An image representation of the MoG background model
        /// </summary>
        public Image<Bgr, float> BackgroundImage()
        {
            var background = new Image<Bgr, float>(Width, Height);
            for(var i=0; i<Width;i++)
                for (var j = 0; j < Height; j++)
                {
                    var pixel = MmImage[i, j].SampleBackground();
                    background.Data[j, i, 0] = pixel[0];
                    background.Data[j, i, 1] = pixel[1];
                    background.Data[j, i, 2] = pixel[2];
                }

            return background;
        }

        /// <summary>
        /// An image representation of MoG foreground pixels
        /// </summary>
        public Image<Gray, bool> ForegroundMask(Image<Bgr, Byte> frame)
        {
            var foreground = new Image<Gray, bool>(Width, Height);
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    var sample = new int[3] { frame.Data[j, i, 0] , frame.Data[j, i, 1] , frame.Data[j, i, 2] };
                    foreground.Data[j, i, 0] = !MmImage[j, i].IsForegroundSample(sample);
                }

            return foreground;
        }

    }

    /// <summary>
    /// A multidimensional Gaussian distribution
    /// </summary>
    public class GaussianComponent
    {
        /// <summary>
        /// Single, multidimensional Gaussian
        /// </summary>
        private readonly Normal[] _mGaussian; 
        public int Dimensionality => _mGaussian.Length;
        public double Variance => Math.Sqrt(_mGaussian.Aggregate(0.0, (d, normal) => d + Math.Pow(normal.Variance, 2)));

        /// <summary>
        /// Multidimensional Gaussian distribution: use this constructor when distribution parameters are unknown
        /// </summary>
        /// <param name="numDimensions">Dimensionality of distribution</param>
        /// <param name="defaultMean">Initialization value for distribution mean</param>
        /// <param name="defaultVariance">Initialization value for distribution variance</param>
        public GaussianComponent(int numDimensions, double defaultMean, double defaultVariance)
        {
            _mGaussian = new Normal[numDimensions];
            for (var i = 0; i < numDimensions; i++)
                _mGaussian[i] = new Normal(defaultMean, defaultVariance);
        }

        /// <summary>
        /// Multidimensional Gaussian: use this constructor to create a component based on a sample
        /// </summary>
        /// <param name="sample">sample from which to extract initialization means</param>
        /// <param name="defaultVariance"></param>
        public GaussianComponent(double[] sample, double defaultVariance)
        {
            _mGaussian = new Normal[sample.Length];
            for (var i = 0; i < sample.Length; i++)
                _mGaussian[i] = new Normal(sample[i], defaultVariance);
        }

        /// <summary>
        /// Percentage of observations accounted for by this component
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Update distribution properties assuming that this sample originated from this Gaussian
        /// </summary>
        /// <param name="sample">Pixel RGB values</param>
        /// <param name="alpha">Update rate</param>
        public void UpdateParameters(double[] sample, double alpha)
        {
            var gaussianWithSampleValues = AssociateDimensionsWithSamples(sample);
            //var p = alpha*SampleProbability(sample);
            var p = alpha; // Deviating from the Stauffer and Grimson implementation. Need to update faster.

            foreach (var gaussianWithSample in gaussianWithSampleValues)
            {
                var singleDimSample = gaussianWithSample.Item1;
                var singleDimMean = gaussianWithSample.Item2.Mean;
                var singleDimVariance = gaussianWithSample.Item2.Variance;

                gaussianWithSample.Item2.Mean = (1 - p)* singleDimMean + p * singleDimSample;
                gaussianWithSample.Item2.Variance = (1 - p)*singleDimVariance +
                                                    p*(singleDimSample - singleDimMean)*
                                                    (singleDimSample - singleDimMean);


            }

            Weight = (1 - alpha) * Weight + alpha;
        }

        /// <summary>
        /// Calculate probability density of Gaussian for this sample point
        /// </summary>
        /// <param name="sample"></param>
        /// <returns>Probability</returns>
        private double SampleProbability(double[] sample)
        {
            var gaussianWithSampleValues = AssociateDimensionsWithSamples(sample);
            var probability = gaussianWithSampleValues.Aggregate(1.0, (p, tuple) => p*tuple.Item2.Density(tuple.Item1));
            return probability;
        }

        /// <summary>
        /// Zip a multidimensional sample to the Gaussian distribution for dimension
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        private List<Tuple<double, Normal>> AssociateDimensionsWithSamples(double[] sample)
        {
            var sampleList = sample.ToList();
            return sampleList.Zip(_mGaussian, (d, normal) => new Tuple<double, Normal>(d, normal)).ToList();
        }

        /// <summary>
        /// Check if a sample is within some mahalanobis distance from this distribution
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="stdDevThreshold"></param>
        /// <returns></returns>
        public bool SampleMatch(double[] sample, double stdDevThreshold = 2.5)
        {
           var mahalanobis = AssociateDimensionsWithSamples(sample).Aggregate(0.0, (p, tuple) => p + (tuple.Item1 - tuple.Item2.Mean)/tuple.Item2.StdDev )/sample.Length;
            if (mahalanobis > stdDevThreshold)
                return false;

            return true;
        }

        public int[] Sample()
        {
            return _mGaussian.Select(x => (int) x.Mean).ToArray();
        }
    }

    /// <summary>
    /// A distribution composed of K Gaussians. Used to model pixel-process RGB values in order to perform background subtraction.
    /// </summary>
    public class MixtureModel
    {
        /// <summary>
        /// List of gaussian components present in the mixture
        /// </summary>
        private readonly List<GaussianComponent> _components = new List<GaussianComponent>();
        private int NumComponents => _components.Count;
        private int Dimensionality => _components.First().Dimensionality;

        /// <summary>
        /// Update rate for learning
        /// </summary>
        private const double Alpha = 0.3;

        /// <summary>
        /// Minimum proportion of samples that should be accounted for by the background model
        /// </summary>
        private const double T = 0.5;

        /// <summary>
        /// Initial variance for components on first observation
        /// </summary>
        private const double DefaultVariance = 50;

        /// <summary>
        /// Online Mixture-of-Gaussians incremental calculation,  
        /// taken from Stauffer and Grimson: "Adaptive background mixture models for real-time tracking"
        /// </summary>
        /// <param name="sample">Single multidimensional sample</param>
        public void TrainIncremental(int[] sample)
        {
            var dSample = sample.Select(Convert.ToDouble).ToArray();

            if (MatchesAny(sample))
                NearestMatchOrNull(sample).UpdateParameters(dSample, Alpha);
            else
                AddNewComponent(sample);
        }

        /// <summary>
        /// Determines whether a sample is likely to have originated from one of the background components
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public bool IsForegroundSample(int[] sample)
        {
            var dSample = sample.Select(Convert.ToDouble).ToArray();
            var bgComponents = BackgroundComponents();
            return bgComponents.Any(c => c.SampleMatch(dSample));
        }

        /// <summary>
        /// Determines whether a sample is likely to have originated from any of the model components
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        private bool MatchesAny(int[] sample)
        {
            var dSample = sample.Select(Convert.ToDouble).ToArray();
            return _components.Any(c => c.SampleMatch(dSample));
        }

        private GaussianComponent NearestMatchOrNull(int[] sample)
        {
            var dSample = sample.Select(Convert.ToDouble).ToArray();
            for(var i=0; i < _components.Count; i++)
                if (_components.ElementAt(i).SampleMatch(dSample))
                    return _components.ElementAt(i);

            return null;
        }

        /// <summary>
        /// Model components heuristically determined to represent the background. 
        /// </summary>
        /// <returns>Components heuristically determined to represent the background</returns>
        private List<GaussianComponent> BackgroundComponents()
        {
            _components.Sort(
                 delegate (GaussianComponent a, GaussianComponent b)
                 {
                     if (a.Weight/a.Variance > b.Weight/b.Variance)
                         return 1;
                     else
                         return -1;
                 });
            _components.Reverse(); //Sorted in order of decreasing background-ness

            var bComponents = new List<GaussianComponent>();
            var proportion = 0.0;
            for (var i = 0; i < _components.Count; i++)
            {
                proportion += _components.ElementAt(i).Weight;
                bComponents.Add(_components.ElementAt(i));
                if (proportion >= T)
                    break;
            }

            return bComponents;
        } 

        /// <summary>
        /// Create new multidimensional Gaussian distribution component based on a sample, add to components list
        /// </summary>
        /// <param name="sample"></param>
        private void AddNewComponent(int[] sample)
        {
            var dSample = sample.Select(Convert.ToDouble).ToArray();
            var newComponent = new GaussianComponent(dSample, DefaultVariance);
            _components.Add(newComponent);
        }

        public int[] SampleBackground()
        {
            var dominantBackground = BackgroundComponents().First();
            return dominantBackground.Sample();
        }
    }
}
