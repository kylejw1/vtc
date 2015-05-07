using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using VTC.Kernel.EventConfig;
using VTC.Kernel.Settings;

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

        #region Static colors

        private static readonly Bgr _whiteColor = new Bgr(Color.White);
        private static readonly Gray _tempMovementGray = new Gray(0);
        private static readonly Bgr _blobCenterColor = new Bgr(255.0, 255.0, 255.0);
        private static readonly Bgr _greenColor = new Bgr(Color.Green);
        private static readonly Bgr _stateColorGreen = new Bgr(0.0, 255.0, 0.0);
        private static readonly Bgr _stateColorRed = new Bgr(0.0, 0.0, 255.0);
        private readonly Gray _thresholdColor;
        private static readonly Gray _ceilingColor = new Gray(255);

        #endregion

        //************* Abstract methods ****************
        protected abstract void UpdateChildClassStats(List<Vehicle> deleted);

        //************* Main image variables ***************
        private Image<Bgr, Byte> Frame; //current Frame from camera
        private Image<Bgr, float> Color_Difference; //Difference between the two frames
        private Image<Bgr, float> ROI_image; //Area occupied by traffic
        private readonly int Width;
        private readonly int Height;
        
        public Image<Gray, Byte> Movement_Mask { get; private set; } //Thresholded, b&w movement mask
        public Image<Gray, Byte> Movement_MaskMoG { get; private set; } //Thresholded, b&w movement mask
        public Image<Bgr, float> Color_Background { get; private set; } //Average Background being formed
        public Image<Bgr, Byte> Training_Image { get; private set; } //Image to be exported for training set

        public Image<Bgr, Byte>[] MeanImages; // Mixture of Gaussian parameters
        public Image<Gray, Byte>[] VarianceImages; 
        public Image<Gray, Byte>[] WeightImages;

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

                // update PerCar

                    // The sum of moving area is calculated by adding all pixel values across the image 
                    // in the movement/foreground image after multiplication by the intersection ROI
                    // Total value for a white pixel = 3x255 = 765

                PerCar = Math.PI * value * value * (3 * 255);
            }
        }
        private double _carRadius;

        private double PerCar
        {
            get { return _perCar; }
            set
            {
                _perCar = Math.Max(value, Settings.PerCarMinimum);
            }
        }
        private double _perCar;

        public double NoiseMass { get; set; }

        //************* Event detection parameters ***************
        private Dictionary<RegionTransition, string> Events;          //Map from region transitions to event types

        //************* Rendering parameters ***************  
        double velocity_render_multiplier = 1.0; //Velocity is multiplied by this quantity to give a line length for rendering
        bool render_clean = true;                //Don't draw velocity vector, use fixed-size object circles. Should add this as checkbox to UI.

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

                ROI_image = RegionConfiguration.RoiMask.GetMask(Width, Height, _whiteColor);

                ResetStats();
            }
        }

        private EventConfig.EventConfig _eventConfiguration = null;

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

                ROI_image = RegionConfiguration.RoiMask.GetMask(Width, Height, _whiteColor);
            }
        }

        public List<Vehicle> CurrentVehicles
        {
            get { return MHT.CurrentVehicles; }
        }

        protected Vista(ISettings settings, int width, int height)
        {
            Settings = settings;
            Width = width;
            Height = height;

            RegionConfiguration = RegionConfig.RegionConfig.Load(settings.RegionConfigPath);
            if (null == RegionConfiguration)
                RegionConfiguration = new RegionConfig.RegionConfig();

            EventConfiguration = new EventConfig.EventConfig();

            LastDetectionCount = 0;
            RawMass = 0;

            var vf = new VelocityField(50, 50, Width, Height); 

            _thresholdColor = new Gray(Settings.ColorThreshold);
            MHT = new MultipleHypothesisTracker(settings, vf);

            CarRadius = Settings.CarRadius;
            NoiseMass = Settings.NoiseMass;

            BackgroundUpdateMoG = new Image<Bgr, float>(Width, Height);
        }

        public void DrawVelocityField<TColor, TDepth>(Emgu.CV.Image<TColor, TDepth> image, TColor color, int thickness) 
            where TColor : struct, IColor 
            where TDepth : new()
        {
            if (null == MHT)
                return;

            MHT.VelocityField.Draw(image, color, thickness);
        }

        public virtual void ResetStats()
        {
            TotalDeleted = 0;
        }

        public virtual string GetStatString()
        {
            return "Vehicles detected: " + TotalDeleted;               
        }

        private int numProcessedFrames = 0;
        private int backgroundFrameDownsampling = 100;
        public void Update(Image<Bgr, Byte> newFrame)
        {
            numProcessedFrames++;
            Frame = newFrame;

            if (Color_Background == null) //we need at least one frame to initialize the background
            {
                Color_Background = newFrame.Convert<Bgr, float>();
            }
            else
            {
                Movement_Mask = MovementMask(newFrame, Color_Background);
                //if (BackgroundUpdateMoG != null)
                //    Movement_MaskMoG = MovementMask(newFrame, BackgroundUpdateMoG);

                UpdateBackground(newFrame);

                //if(numProcessedFrames%backgroundFrameDownsampling == 0)
                //UpdateBackgroundMoG(newFrame);

                Training_Image = newFrame.And(Movement_Mask.Convert<Bgr, byte>());

                //var measurements = FindBlobCenters(newFrame, count);
                var measurements = FindClosedBlobCenters(newFrame);
                MHT.Update(measurements);

                // First update base class stats
                UpdateVistaStats(MHT.DeletedVehicles);

                // Now update child class specific stats
                UpdateChildClassStats(MHT.DeletedVehicles);
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
                Color_Background.RunningAvg(BackgroundUpdate, Settings.Alpha);
            }
        }

        //Calculate background using Mixture of Gaussians
        private const int MaxSamples = 100;
        public Image<Bgr, float> BackgroundUpdateMoG;
        public Image<Bgr, float> MovementImageMoG;
        public Queue<Image<Bgr, float>> FrameSamples = new Queue<Image<Bgr, float>>(MaxSamples);
        private void UpdateBackgroundMoG(Image<Bgr, Byte> frame)
        {
            Image<Bgr, float> newImageSample = frame.Convert<Bgr, float>();
            FrameSamples.Enqueue(newImageSample);

            int numSamples = FrameSamples.Count();
            if (numSamples == MaxSamples)
            {
                for (int i = 0; i < newImageSample.Width; i++)
                    for (int j = 0; j < newImageSample.Height; j++)
                    {
                        var samplePoints = FrameSamples.Select(x => new int[3] { Convert.ToInt32(x.Data[j, i, 0]), Convert.ToInt32(x.Data[j, i, 1]), Convert.ToInt32(x.Data[j, i, 2]) }).ToArray();

                        if(i==336 && j == 185)
                            Console.WriteLine("Here it is");

                        MixtureModel mm = new MixtureModel(samplePoints);
                        mm.Train();
                        BackgroundUpdateMoG.Data[j, i, 0] = (float)mm.Means[0][0];
                        BackgroundUpdateMoG.Data[j, i, 1] = (float)mm.Means[0][1];
                        BackgroundUpdateMoG.Data[j, i, 2] = (float)mm.Means[0][2];
                    }

                FrameSamples.Clear();
            }
        }

        

        public void InitializeBackground(Image<Bgr, Byte> frame)
        {
            using (Image<Bgr, float> BackgroundUpdate = frame.Convert<Bgr, float>())
            {
                Color_Background.RunningAvg(BackgroundUpdate, 1.0);
            }
        }


        private Image<Gray, Byte> MovementMask(Image<Bgr,Byte> frame, Image<Bgr, float> background)
        {
            Image<Bgr, float> colorDifference = background.AbsDiff(frame.Convert<Bgr, float>());
            Image<Bgr, float> maskedDifference = colorDifference.And(ROI_image);  
            Image<Gray, Byte> movementMask = maskedDifference.Convert<Gray, Byte>();

            movementMask._Erode(1);
            movementMask._Dilate(1);
            movementMask._Erode(1);
            movementMask._Dilate(1);
            movementMask._SmoothGaussian(5, 5, 1, 1);
            movementMask._ThresholdBinary(_thresholdColor, _ceilingColor);

            return movementMask;
        }

        private Bgr GetBlobColour(Image<Bgr, Byte> frame, double x, double y, double radius)
        {
            var mask = new Image<Gray, Byte>(frame.Size);
            var center = new PointF((float)x, (float)y);
            var circle = new CircleF(center, (float)radius);
            mask.Draw(circle, new Gray(255), 0);

            return frame.GetAverage(mask);
        }

        private Measurements[] FindBlobCenters(Image<Bgr, Byte> frame, int count)
        {
			Measurements[] coordinates;
            using (Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone())
            {
                coordinates = new Measurements[count];
                for (int detection_count = 0; detection_count < count; detection_count++)
                {
                    double[] minValues;
                    double[] maxValues;
                    Point[] minLocations;
                    Point[] maxLocations;
                    tempMovement_Mask.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                    int[] maxLocation = new int[] { maxLocations[0].X, maxLocations[0].Y };
                    tempMovement_Mask.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), Settings.CarRadius), _tempMovementGray, 0);
					
					var x = maxLocation[0];
					var y = maxLocation[1];
					var colour = GetBlobColour(frame, x, y, 3.0);
					var coords = new Measurements() { x = x, y = y, red = colour.Red, green = colour.Green, blue = colour.Blue };
					coordinates[detection_count] = coords;

                    //Do this last so that it doesn't interfere with color sampling
					frame.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), 1), new Bgr(255.0, 255.0, 255.0), 1);
                }
            }
            return coordinates;
        }

        private Measurements[] FindClosedBlobCenters(Image<Bgr, Byte> frame)
        {
            Measurements[] coordinates;

            using (Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone())
            {
                Emgu.CV.Cvb.CvBlobs resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
                Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
                bDetect.Detect(tempMovement_Mask, resultingImgBlobs);
                int numWebcamBlobsFound = resultingImgBlobs.Count;
                if (numWebcamBlobsFound > Settings.MaxObjectCount)
                    numWebcamBlobsFound = Settings.MaxObjectCount;

                BlobAreaComparer areaComparer = new BlobAreaComparer();
                SortedList<CvBlob, int> blobsWithArea = new SortedList<CvBlob, int>(areaComparer);
                foreach (Emgu.CV.Cvb.CvBlob targetBlob in resultingImgBlobs.Values)
                    if(targetBlob.Area > 25)
                        blobsWithArea.Add(targetBlob, targetBlob.Area);

                numWebcamBlobsFound = blobsWithArea.Count();

                coordinates = new Measurements[numWebcamBlobsFound];
                for(int i=0; i<numWebcamBlobsFound; i++)
                {
                    CvBlob targetBlob = blobsWithArea.ElementAt(i).Key;
                    double x = targetBlob.Centroid.X;
                    double y = targetBlob.Centroid.Y;
                    var colour = GetBlobColour(frame, x, y, 3.0);
                    var coords = new Measurements()
                    {
                        x = x,
                        y = y,
                        red = colour.Red,
                        green = colour.Green,
                        blue = colour.Blue
                    };
                    coordinates[i] = coords;
                    //Do this last so that it doesn't interfere with color sampling
                    frame.Draw(new CircleF(new PointF((float) x, (float) y), 1), new Bgr(255.0, 255.0, 255.0), 1);
                }
                
            }
            return coordinates;
        }

        public Image<Bgr, float> GetBackgroundImage(bool drawMaskPolygons)
        {
            if (drawMaskPolygons)
            {
                Image<Bgr, float> Overlay = new Image<Bgr, float>(Width, Height, _greenColor);
                Overlay = Overlay.And(ROI_image);
                Overlay.Acc(Color_Background);

                return Overlay;
            }
            else
            {
                return Color_Background;
            }

        }

        public Image<Bgr, byte> GetCurrentStateImage()
        {
            var frame = Frame.Clone();

            var vehicles = MHT.CurrentVehicles;

            vehicles.ForEach(delegate(Vehicle vehicle)
            {

                var lastState = vehicle.state_history.Last();

                float x = (float)lastState.x;
                float y = (float)lastState.y;

                var validation_region_deviation = MHT.ValidationRegionDeviation;

                float radius = validation_region_deviation * ((float)Math.Sqrt(Math.Pow(lastState.cov_x, 2) + (float)Math.Pow(lastState.cov_y, 2)));
                if (radius < 2.0)
                    radius = (float)2.0;

                float vx_render = (float)(velocity_render_multiplier * lastState.vx);
                float vy_render = (float)(velocity_render_multiplier * lastState.vy);


                if (render_clean)
                {
                    frame.Draw(new CircleF(new PointF(x, y), 10), new Bgr(vehicle.state_history.Last().blue, vehicle.state_history.Last().green, vehicle.state_history.Last().red), 2);
                    frame.Draw(new CircleF(new PointF(x, y), 2), _stateColorGreen, 1);
                }
                else
                {
                    frame.Draw(new CircleF(new PointF(x, y), radius), _stateColorGreen, 1);
                    frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), _stateColorRed, 1);
                }

            }
           );

            return frame;
        }
    }
}

class BlobAreaComparer : IComparer<CvBlob>
{
     public int Compare(CvBlob x, CvBlob y)
     {
         return (x.Area < y.Area) ? 1 : -1;
     }
}
        