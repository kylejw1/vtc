using System;
using System.Collections.Generic;
using System.Drawing;
using VTC.Kernel;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using VTC.Kernel.EventConfig;
using System.Collections;
using Emgu.CV.CvEnum;
using GeoAPI.Geometries;
using MathNet.Numerics.Interpolation.Algorithms;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using VTC.Common;
using Point = System.Drawing.Point;
using NLog;


namespace VTC.Kernel.Vistas
{
    /// <summary>
    /// A Vista represents the statistics and analysis of the video stream.
    /// It is intended to be extended by classes for specific use cases such
    /// as intersections, parking lots, etc.
    /// "a mental view of a succession of remembered or anticipated events"
    /// </summary>
    public abstract class Vista
    {
        protected ISettings Settings { get; private set; }
        private static readonly Logger _logger = LogManager.GetLogger("vista");

        public delegate string GetSource();
        public GetSource GetCameraSource;

        #region Static colors

        private static readonly Bgr WhiteColor = new Bgr(Color.White);
        private static readonly Gray TempMovementGray = new Gray(0);
        private static readonly Bgr GreenColor = new Bgr(Color.Green);
        private static readonly Bgr StateColorGreen = new Bgr(0.0, 255.0, 0.0);
        private static readonly Bgr StateColorRed = new Bgr(0.0, 0.0, 255.0);
        private readonly Gray _thresholdColor;
        private static readonly Gray CeilingColor = new Gray(255);

        #endregion

        //************* Abstract methods ****************
        protected abstract void UpdateChildClassStats(List<Vehicle> deleted);

        //************* Main image variables ***************
        public Image<Bgr, Byte> _frame; //current Frame from camera
        public Image<Bgr, Byte> _correctedFrame; //color-corrected current frame from camera (possibly rotation-corrected later)
        public Image<Bgr, float> _roiImage; //Area occupied by traffic
        private readonly int _width;
        private readonly int _height;

        public Measurement[] MeasurementsArray;
        public Queue<Measurement[]> MeasurementArrayQueue;
        
        public Image<Gray, byte> Movement_Mask { get; private set; } //Thresholded, b&w movement mask
        public Image<Bgr, float> ColorBackground { get; private set; } //Average Background being formed
        public Image<Bgr, byte> TrainingImage { get; private set; } //Image to be exported for training set

        public Image<Bgr, byte>[] MeanImages; // Mixture of Gaussian parameters
        public Image<Gray, byte>[] VarianceImages; 
        public Image<Gray, byte>[] WeightImages;

        public MoGBackground MoGBackgroundSingleton; 

        //************* Optical flow variables ***************
        public double[][][] OpticalFlow; // LxWx2. Vector field where each pixel represents optical flow direction.
        private const int OpticalFlowDownsample = 1;
        private const int OpticalFlowLimit = 8;
        public Image<Bgr, byte> lastFrame; //For calculating single-frame optical flow 
        public bool DisableOpticalFlow = false;

        //*************************** Image processing controls ***************************
        public bool EnableMoG; //If true, MoG background is used instead of rolling background

    //************* Object detection parameters ***************  
        public double RawMass { get; private set; }
        public int LastDetectionCount { get; private set; }

        //TODO: Decide later where this should go (could be here)
        //private 

        public double CarRadius
        {
            get { return _carRadius; }
            set
            {
                _carRadius = value;
            }
        }
        private double _carRadius;

        //************* Event detection parameters ***************
        private Dictionary<RegionTransition, string> Events;          //Map from region transitions to event types

        //************* Rendering parameters ***************  
        double velocity_render_multiplier = 1.0; //Velocity is multiplied by this quantity to give a line length for rendering
        bool render_clean = true;                //Don't draw velocity vector, use fixed-size object circles. Should add this as checkbox to UI.
        public bool hide_trackers = false;              //Don't draw tracker circles at all

        private readonly MultipleHypothesisTracker MHT;

        private int TotalDeleted = 0;

        private RegionConfig.RegionConfig _regionConfiguration = null;
        public RegionConfig.RegionConfig RegionConfiguration
        {
            get
            {
                return _regionConfiguration;
            }
            set
            {
                if (_regionConfiguration == value) return;

                _regionConfiguration = value;

                _roiImage = RegionConfiguration.RoiMask.GetMask(_width, _height, WhiteColor);

                ResetStats();
            }
        }

        private EventConfig.EventConfig _eventConfiguration;

        public EventConfig.EventConfig EventConfiguration
        {
            get
            {
                return _eventConfiguration;
            }
            set
            {
                if (_eventConfiguration == value) return;

                _eventConfiguration = value;

                _roiImage = RegionConfiguration.RoiMask.GetMask(_width, _height, WhiteColor);
            }
        }

        public List<Vehicle> CurrentVehicles
        {
            get { return MHT.CurrentVehicles; }
        }

        protected Vista(ISettings settings, int width, int height)
        {
            Settings = settings;
            _width = width;
            _height = height;

            string configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                    "\\VTC\\regionConfig.xml";
            RegionConfiguration = RegionConfig.RegionConfig.Load(configFilePath);
            if (null == RegionConfiguration)
                RegionConfiguration = new RegionConfig.RegionConfig();

            EventConfiguration = new EventConfig.EventConfig();

            LastDetectionCount = 0;
            RawMass = 0;

            var vf = new VelocityField(Settings.VelocityFieldResolution, Settings.VelocityFieldResolution, _width, _height); 

            _thresholdColor = new Gray(Settings.ColorThreshold);
            MHT = new MultipleHypothesisTracker(settings, vf);

            CarRadius = Settings.CarRadius;

            MoGBackgroundSingleton = new MoGBackground(_width, _height, _roiImage);

            MeasurementArrayQueue = new Queue<Measurement[]>(900);
            OpticalFlow = new double[_width][][];
            for (int i = 0; i < _width; i++)
            {
                OpticalFlow[i] = new double[_height][];
                for (int j = 0; j < _height; j++)
                    OpticalFlow[i][j] = new double[2] { 0, 0 };
            }
                
        }

        /// <summary>
        /// Write log message.
        /// </summary>
        /// <param name="logLevel">Log message severity.</param>
        /// <param name="format">Message format.</param>
        /// <param name="args">Format argument.</param>
        private static void Log(LogLevel logLevel, string format, params object[] args)
        {
            Console.WriteLine(format, args);
            _logger.Log(logLevel, format, args);
        }

        private static void Log(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void DrawVelocityField<TColor, TDepth>(Emgu.CV.Image<TColor, TDepth> image, TColor color, int thickness) 
            where TColor : struct, IColor 
            where TDepth : new()
        {
            if (null == MHT)
                return;

            MHT.VelocityField.Draw(image, color, thickness);
        }

        public Emgu.CV.Image<Gray, Byte> VelocityProjection()
        {
            if (null == MHT)
                return null;

            return MHT.VelocityField.ProjectedPointsImage;
        }

        public virtual void ResetStats()
        {
            TotalDeleted = 0;
        }

        public virtual string GetStatString()
        {
            return "Objects detected: " + TotalDeleted;               
        }

        private int numProcessedFrames = 0;
        public void Update(Image<Bgr, Byte> newFrame)
        {
            try
            {
                numProcessedFrames++;
                _frame = newFrame;

                if (ColorBackground == null)
                    ColorBackground = newFrame.Convert<Bgr, float>();

                Movement_Mask = EnableMoG ? MovementMaskMoG(newFrame) : MovementMask(newFrame, ColorBackground);

                UpdateBackground(newFrame);

                if (!DisableOpticalFlow)
                    UpdateOpticalFlow(newFrame);

                if (numProcessedFrames % Settings.MoGUpdateDownsampling == 0 && EnableMoG)
                    UpdateBackgroundMoG(newFrame);

                TrainingImage = newFrame.And(Movement_Mask.Convert<Bgr, byte>());

                MeasurementsArray = FindClosedBlobCenters(newFrame, Movement_Mask, Settings);
                MeasurementArrayQueue.Enqueue(MeasurementsArray);
                while (MeasurementArrayQueue.Count > 300)
                    MeasurementArrayQueue.Dequeue();

                MHT.Update(MeasurementsArray);
                
                // First update base class stats
                UpdateVistaStats(MHT.DeletedVehicles);

                // Now update child class specific stats
                UpdateChildClassStats(MHT.DeletedVehicles);

                lastFrame = newFrame;
            }
            catch (Exception e)
            {
#if DEBUG
                throw;           
#else
                _logger.Log(LogLevel.Error, "In Vista:Update(), " + e.Message);
#endif
            }   
        }

        private void UpdateVistaStats(List<Vehicle> deleted)
        {
            if (null != deleted)
                TotalDeleted += deleted.Count();
        }

        private void UpdateBackground(Image<Bgr, Byte> frame)
        {
            using (Image<Bgr, float> BackgroundUpdate = frame.Convert<Bgr, float>())
            {
                ColorBackground.AccumulateWeighted(BackgroundUpdate, Settings.Alpha);
            }
        }

        private void UpdateOpticalFlow(Image<Bgr, Byte> frame)
        {
            if (lastFrame != null)
            {
                if (CudaInvoke.HasCuda)
                {
                    
                    //using (var of = new CudaFarnebackOpticalFlow())
                    using (var of = new CudaBroxOpticalFlow())
                    {
                        var singleChanFrame = frame.Convert<Gray, byte>();
                        var singleChanLastFrame = lastFrame.Convert<Gray, byte>();
                        Mat flow = new Mat();
                        using (CudaImage<Gray, float> prevGpu = new CudaImage<Gray, float>(singleChanLastFrame.Convert<Gray, float>()))
                        using (CudaImage<Gray, float> currGpu = new CudaImage<Gray, float>(singleChanFrame.Convert<Gray, float>()))
                        using (GpuMat flowGpu = new GpuMat())
                        {
                            of.Calc(prevGpu, currGpu, flowGpu);

                            flowGpu.Download(flow);
                        }
                        
                        var channels = flow.Split();
                        var ch1 = channels[0];
                        var ch2 = channels[1];
                        float[, ,] xarr = new float[frame.Height, frame.Width, 1];
                        ch1.CopyTo(xarr);
                        float[, ,] yarr = new float[frame.Height, frame.Width, 1];
                        ch2.CopyTo(yarr);

                        for (int i = 0; i < _width; i++)
                            for (int j = 0; j < _height; j++)
                                Array.Clear(OpticalFlow[i][j], 0, 2); 

                        for (int i = 0; i < _width; i += OpticalFlowDownsample)
                            for (int j = 0; j < _height; j += OpticalFlowDownsample)
                            {
                                //if (Movement_Mask.Data[j, i, 0] > 0)
                                {
                                    if (i > OpticalFlowLimit && i < _width - OpticalFlowLimit)
                                        if (j > OpticalFlowLimit && j < _height - OpticalFlowLimit)
                                        {
                                            //var lsse = VTC.Kernel.OpticalFlow.LowestSSEPair(frame, lastFrame, i, j, 1,
                                            //    20, 8);
                                            //OpticalFlow[i][j][0] = lsse.XOffset;
                                            //OpticalFlow[i][j][1] = lsse.YOffset;
                                            OpticalFlow[i][j][0] = xarr[j, i, 0];
                                            OpticalFlow[i][j][1] = yarr[j, i, 0];
                                        }
                                }
                            }
                    }
                }
            }
        }

        private void UpdateBackgroundMoG(Image<Bgr, Byte> frame)
        {
            //MoGBackgroundSingleton.TryUpdatingAsync(frame);
            MoGBackgroundSingleton.Update(frame);
        }
       
        public void InitializeBackground(Image<Bgr, Byte> frame)
        {
            using (Image<Bgr, float> BackgroundUpdate = frame.Convert<Bgr, float>())
            {
                ColorBackground.AccumulateWeighted(BackgroundUpdate, 1.0);
            }
        }

        /// <summary>
        /// Use Gaussian probabilities to decide whether a pixel is a sample from background or foreground model
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private Image<Gray, Byte> MovementMaskMoG(Image<Bgr, Byte> frame)
        {
            //Evaluate current frame
            Image<Gray, Byte> mogMask = new Image<Gray, Byte>(frame.Width, frame.Height);

            //ApplyColorCorrection(frame,MoGBackgroundSingleton.BackgroundImage());

            for (int i=0;i<frame.Width;i++)
                for(int j=0; j<frame.Height; j++)
                {
                    if (_roiImage.Data[j, i,0] != 0)
                    {
                        var sample = new int[] { frame.Data[j, i, 0], frame.Data[j, i, 1], frame.Data[j, i, 2] };
                        if (!MoGBackgroundSingleton.MmImage[i, j].IsBackgroundSample(sample))
                            mogMask.Data[j, i, 0] = byte.MaxValue;
                        else
                            mogMask.Data[j, i, 0] = 0;
                    }
                    
                }

            return mogMask;
        }

        //TODO: Clea this function up, too many things happening
        private Image<Gray, Byte> MovementMask(Image<Bgr,Byte> frame, Image<Bgr, float> background)
        {
            //ApplyColorCorrection(frame, background);

            Image<Bgr, float> colorDifference = background.AbsDiff(frame.Convert<Bgr, float>());
            Image<Bgr, float> maskedDifference = colorDifference.And(_roiImage);  
            Image<Gray, Byte> movementMask = maskedDifference.Convert<Gray, Byte>();

            movementMask._SmoothGaussian(5, 5, 1, 1);
            movementMask._ThresholdBinary(_thresholdColor, CeilingColor);

            return movementMask;
        }

        //TODO: rewrite this method (efficiency, correctness)
        /// <summary>
        /// Sample evenly-spaced points in image and compare to background. Determine average error in % on each channel (R,G,B). Apply inverse
        /// tranform to bring corrected frame nearer to background. 
        /// </summary>
        /// <param name="frame">Input frame</param>
        /// <param name="background">Reference for correction</param>
        private void ApplyColorCorrection(Image<Bgr, byte> frame, Image<Bgr, float> background)
        {
            throw new NotImplementedException();
            //Adjust incoming frame by white-balancing to match background image
            Image<Bgr, float> whiteBalancedFrame = new Image<Bgr, float>(frame.Width, frame.Height);
            int numColorSamples = 20;
            int sampleSpacing = (Math.Min(frame.Width, frame.Height)/numColorSamples) - 1;
            double balanceMagnitudeThreshold = 0.5;
            double bFactorAvg = 0;
            double gFactorAvg = 0;
            double rFactorAvg = 0;
            int numGoodSamples = 0;
            _correctedFrame = new Image<Bgr, byte>(frame.Width, frame.Height);

            for (int i = 0; i < numColorSamples; i++)
            {
                try
                {
                    double bFactor = (double) frame.Data[sampleSpacing*i, sampleSpacing*i, 0]/
                                     ((int) background.Data[sampleSpacing*i, sampleSpacing*i, 0]);
                    double gFactor = (double) frame.Data[sampleSpacing*i, sampleSpacing*i, 1]/
                                     ((int) background.Data[sampleSpacing*i, sampleSpacing*i, 1]);
                    double rFactor = (double) frame.Data[sampleSpacing*i, sampleSpacing*i, 2]/
                                     ((int) background.Data[sampleSpacing*i, sampleSpacing*i, 2]);
                    double magnitude =
                        Math.Sqrt(Math.Pow(Math.Abs(1.0 - bFactor), 2) + Math.Pow(Math.Abs(1.0 - gFactor), 2) +
                                  Math.Pow(Math.Abs(1.0 - rFactor), 2));
                    if (!double.IsNaN(magnitude) && !double.IsInfinity(magnitude) && magnitude < balanceMagnitudeThreshold)
                    {
                        bFactorAvg += bFactor;
                        gFactorAvg += gFactor;
                        rFactorAvg += rFactor;
                        numGoodSamples++;
                    }
                }
                catch (DivideByZeroException)
                {
                    //No big deal
                }
            }

            if (numGoodSamples >= 1)
            {
                bFactorAvg = (double) bFactorAvg/numGoodSamples;
                gFactorAvg = (double) gFactorAvg/numGoodSamples;
                rFactorAvg = (double) rFactorAvg/numGoodSamples;

                double bCorrection = 1.0/bFactorAvg;
                double gCorrection = 1.0/gFactorAvg;
                double rCorrection = 1.0/rFactorAvg;

                for (int i = 0; i < frame.Width; i++)
                    for (int j = 0; j < frame.Height; j++)
                    {
                        double adjustedB = frame.Data[j, i, 0]*bCorrection;
                        double adjustedG = frame.Data[j, i, 1]*gCorrection;
                        double adjustedR = frame.Data[j, i, 2]*rCorrection;
                        double adjustedLimitedB = Math.Min(adjustedB, 255);
                        double adjustedLimitedG = Math.Min(adjustedG, 255);
                        double adjustedLimitedR = Math.Min(adjustedR, 255);
                        frame.Data[j, i, 0] = Convert.ToByte(adjustedLimitedB);
                        frame.Data[j, i, 1] = Convert.ToByte(adjustedLimitedG);
                        frame.Data[j, i, 2] = Convert.ToByte(adjustedLimitedR);

                        _correctedFrame.Data[j, i, 0] = Convert.ToByte(adjustedLimitedB);
                        _correctedFrame.Data[j, i, 1] = Convert.ToByte(adjustedLimitedG);
                        _correctedFrame.Data[j, i, 2] = Convert.ToByte(adjustedLimitedR);
                    }
            }
        }

        private static Bgr GetBlobColour(Image<Bgr, Byte> frame, double x, double y, double radius)
        {
            var mask = new Image<Gray, Byte>(frame.Size);
            var center = new PointF((float)x, (float)y);
            var circle = new CircleF(center, (float)radius);
            mask.Draw(circle, new Gray(255), 0);
            return frame.GetAverage(mask);
        }


        //TODO: rewrite this method (efficiency, correctness)
        private Measurement[] FindBlobCenters(Image<Bgr, Byte> frame, int count)
        {
            throw new NotImplementedException();
			Measurement[] coordinates;
            using (Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone())
            {
                coordinates = new Measurement[count];
                for (int detection_count = 0; detection_count < count; detection_count++)
                {
                    double[] minValues;
                    double[] maxValues;
                    Point[] minLocations;
                    Point[] maxLocations;
                    tempMovement_Mask.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                    int[] maxLocation = new int[] { maxLocations[0].X, maxLocations[0].Y };
                    tempMovement_Mask.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), Settings.CarRadius), TempMovementGray, 0);
					
					var x = maxLocation[0];
					var y = maxLocation[1];
					var colour = GetBlobColour(frame, x, y, 3.0);
					var coords = new Measurement() { X = x, Y = y, Red = colour.Red, Green = colour.Green, Blue = colour.Blue };
					coordinates[detection_count] = coords;

                    //Do this last so that it doesn't interfere with color sampling
					frame.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), 1), new Bgr(255.0, 255.0, 255.0), 1);
                }
            }
            return coordinates;
        }

        private static Measurement[] FindClosedBlobCenters(Image<Bgr, Byte> frame, Image<Gray, byte> movementMask, ISettings settings)
        {
            Measurement[] coordinates = new Measurement[0];
            var w_orig = frame.Width;
            var h_orig = frame.Height;

            using (Image<Gray, Byte> tempMovementMask = movementMask.Clone())
            {
                try
                {
                    var resultingImgBlobs = new CvBlobs();
                    var bDetect = new CvBlobDetector();
                    bDetect.Detect(tempMovementMask, resultingImgBlobs);

                    var areaComparer = new BlobAreaComparer();
                    var BlobsWithArea = new SortedList<CvBlob, int>(areaComparer);

                    foreach (
                        var targetBlob in
                            resultingImgBlobs.Values.Where(targetBlob => targetBlob.Area >= settings.MinObjectSize))
                        BlobsWithArea.Add(targetBlob, targetBlob.Area);

                    int numWebcamBlobsFound = BlobsWithArea.Count();
                    if (numWebcamBlobsFound > settings.MaxTargets)
                        numWebcamBlobsFound = settings.MaxTargets;

                    coordinates = new Measurement[numWebcamBlobsFound];
                    List<Measurement> coordinatesList = new List<Measurement>();
                    for (int i = 0; i < numWebcamBlobsFound; i++)
                    {
                        if (i < settings.MaxTargets)
                        {
                            CvBlob targetBlob = BlobsWithArea.ElementAt(i).Key;
                            var colour = GetBlobColour(frame, targetBlob.Centroid.X, targetBlob.Centroid.Y, 3.0);
                            var coords = new Measurement() { X = targetBlob.Centroid.X, Y = targetBlob.Centroid.Y, Red = colour.Red, Green = colour.Green, Blue = colour.Blue };
                            coordinatesList.Add(coords);
                        }
                    }

                    coordinates = coordinatesList.ToArray();
                    BlobsWithArea.Clear();
                    bDetect.Dispose();
                    resultingImgBlobs.Dispose();
                }
                catch(AccessViolationException e) 
                {
                    //Accessing targetBlob.Centroid occasionally throws AccessViolationException.
                    //This appears to be a problem in either Emgu or OpenCV. TODO: investigate.
                    _logger.Log(LogLevel.Error, "In FindClosedBlobCenters: " + e.Message);
                }   
            }

            return coordinates;
        }

        public Image<Bgr, float> GetBackgroundImage(bool drawMaskPolygons)
        {
            if (drawMaskPolygons)
            {
                Image<Bgr, float> Overlay = new Image<Bgr, float>(_width, _height, GreenColor);
                Overlay = Overlay.And(_roiImage);
                Overlay.Accumulate(ColorBackground);

                return Overlay;
            }
            else
            {
                return ColorBackground;
            }

        }

        public Image<Bgr, byte> GetCurrentStateImage(Image<Bgr, byte> frame)
        {
            var vehicles = MHT.CurrentVehicles;
            Image<Bgr, byte> stateImage = frame.Clone();

            vehicles.ForEach(delegate(Vehicle vehicle)
            {
                var lastState = vehicle.StateHistory.Last();
                float x = (float) lastState.X;
                float y = (float) lastState.Y;

                var validation_region_deviation = MHT.ValidationRegionDeviation;
                float radius = validation_region_deviation*
                               ((float) Math.Sqrt(Math.Pow(lastState.CovX, 2) + (float) Math.Pow(lastState.CovY, 2)));
                if (radius < 2.0)
                    radius = (float) 2.0;

                float vx_render = (float) (velocity_render_multiplier*lastState.Vx);
                float vy_render = (float) (velocity_render_multiplier*lastState.Vy);

                if (!hide_trackers)
                {
                    if (render_clean)
                    {
                        stateImage.Draw(new CircleF(new PointF(x, y), 10),
                            new Bgr(vehicle.StateHistory.Last().Blue, vehicle.StateHistory.Last().Green,
                                vehicle.StateHistory.Last().Red), 2);
                        stateImage.Draw(new CircleF(new PointF(x, y), 2), StateColorGreen, 1);
                    }
                    else
                    {
                        stateImage.Draw(new CircleF(new PointF(x, y), radius), StateColorGreen, 1);
                        stateImage.Draw(
                            new LineSegment2D(new Point((int) x, (int) y),
                                new Point((int) (x + vx_render), (int) (y + vy_render))), StateColorRed, 1);
                    }
                }
            });

            double minDistance = 100;
            foreach (var t in MHT.Trajectories)
            {
                double distance = Math.Sqrt(Math.Pow(t.stateEstimates.First().X - t.stateEstimates.Last().X, 2) +
                                            Math.Pow(t.stateEstimates.First().Y - t.stateEstimates.Last().Y, 2));

                if (distance > minDistance)
                {
                    double ageFactor = 1.0 - (double) (DateTime.Now - t.exitTime).Ticks / (TimeSpan.FromSeconds(3).Ticks);
                    Bgr trajectoryColor = new Bgr(0, 0, 200*ageFactor);
                    Point[] trajectoryRendering = t.stateEstimates.Select(s => new Point((int)s.X, (int)s.Y)).ToArray();
                    stateImage.DrawPolyline(trajectoryRendering, false, trajectoryColor);    
                }
            }
            return stateImage;
        }
    }
}

public class BlobAreaComparer : IComparer<CvBlob>
{
     public int Compare(CvBlob x, CvBlob y)
     {
         return (x.Area < y.Area) ? 1 : -1;
     }
}
        