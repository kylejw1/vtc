using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using TreeLib;
using VTC.Settings;

namespace VTC
{
    /// <summary>
    /// A Vista represents the statistics and analysis of the videa stream.
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
        private readonly Bgr _thresholdColor;

        #endregion
        
        //************* Abstract methods ****************
        protected abstract void UpdateChildClassStats(List<Vehicle> deleted);

        //************* Main image variables ***************
        private Image<Bgr, Byte> Frame;               //current Frame from camera
        private Image<Bgr, float> Color_Difference;   //Difference between the two frames
        private Image<Bgr, float> ROI_image;          //Area occupied by traffic
        private readonly int Width;
        private readonly int Height;
        public Image<Gray, byte> Movement_Mask { get; private set; }      //Thresholded, b&w movement mask
        public Image<Bgr, float> Color_Background { get; private set; }   //Average Background being formed

        //************* Object detection parameters ***************  
        public double RawMass { get; private set; }
        public int LastDetectionCount { get; private set; }

        public double PerCar { get; set; }
        public double NoiseMass { get; set; }

        //************* Event detection parameters ***************
        private Dictionary<RegionTransition, string> Events;          //Map from region transitions to event types

        //************* Rendering parameters ***************  
        double velocity_render_multiplier = 1.0; //Velocity is multiplied by this quantity to give a line length for rendering
        bool render_clean = true;                //Don't draw velocity vector, use fixed-size object circles. Should add this as checkbox to UI.

        private readonly MultipleHypothesisTracker MHT;

        private int TotalDeleted = 0;

        private RegionConfig _regionConfiguration = null;
        public RegionConfig RegionConfiguration
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

        private EventConfig _eventConfiguration = null;

        public EventConfig EventConfiguration
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
            RegionConfiguration = new RegionConfig();
            EventConfiguration = new EventConfig();

            LastDetectionCount = 0;
            RawMass = 0;

            _thresholdColor = new Bgr(Settings.ColorThreshold, Settings.ColorThreshold, Settings.ColorThreshold);
            MHT = new MultipleHypothesisTracker(settings);

            PerCar = Settings.PerCar;
            if (PerCar <= Settings.PerCarMinimum)
                PerCar = Settings.PerCarMinimum;

            NoiseMass = Settings.NoiseMass;
        }

        public virtual void ResetStats()
        {
            TotalDeleted = 0;
        }

        public virtual string GetStatString()
        {
            return "Vehicles detected: " + TotalDeleted;               
        }

        public void Update(Image<Bgr, Byte> newFrame)
        {

            Frame = newFrame;

            if (Color_Background == null) //we need at least one frame to initialize the background
            {
                Color_Background = newFrame.Convert<Bgr, float>();
            }
            else
            {
                int count = CountObjects();

                if (count > Settings.MaxObjectCount)
                    count = Settings.MaxObjectCount;

                UpdateBackground(newFrame);

                Coordinates[] coordinates = FindBlobCenters(newFrame, count);
                MHT.Update(coordinates);

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

        private int CountObjects()
        {
            Color_Difference = Color_Background.AbsDiff(Frame.Convert<Bgr, float>());

            double raw_mass;
            using (Image<Bgr, float> Masked_Difference = Color_Difference.And(ROI_image))
            {
                Masked_Difference._ThresholdBinary(_thresholdColor, _whiteColor);
                Movement_Mask = Masked_Difference.Convert<Gray, Byte>();
                Movement_Mask._SmoothGaussian(5, 5, 1, 1);
                using (Image<Bgr, double> int_img = Masked_Difference.Integral())
                {
                    int lim_x = Masked_Difference.Width - 1;
                    int lim_y = Masked_Difference.Height - 1;
                    raw_mass = (int_img[lim_y, lim_x].Blue + int_img[lim_y, lim_x].Red + int_img[lim_y, lim_x].Green);
                }
            }

            int count = Convert.ToInt32((raw_mass - NoiseMass) / PerCar);
            if (count < 0)
                count = 0;
            if (count > Settings.MaxObjectCount)
                count = Settings.MaxObjectCount;

            return count;
        }

        private Coordinates[] FindBlobCenters(Image<Bgr, Byte> frame, int count)
        {
            Coordinates[] coordinates;
            using (Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone())
            {
                coordinates = new Coordinates[count];
                for (int detection_count = 0; detection_count < count; detection_count++)
                {
                    double[] minValues;
                    double[] maxValues;
                    Point[] minLocations;
                    Point[] maxLocations;
                    tempMovement_Mask.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                    int[] maxLocation = new int[] { maxLocations[0].X, maxLocations[0].Y };
                    tempMovement_Mask.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), Settings.CarRadius), _tempMovementGray, 0);
                    frame.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), 1), _blobCenterColor, 1);
                    coordinates[detection_count].x = maxLocation[0];
                    coordinates[detection_count].y = maxLocation[1];
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

                float x = (float)lastState.coordinates.x;
                float y = (float)lastState.coordinates.y;

                var validation_region_deviation = MHT.ValidationRegionDeviation;

                float radius = validation_region_deviation * ((float)Math.Sqrt(Math.Pow(lastState.cov_x, 2) + (float)Math.Pow(lastState.cov_y, 2)));
                if (radius < 2.0)
                    radius = (float)2.0;

                float vx_render = (float)(velocity_render_multiplier * lastState.vx);
                float vy_render = (float)(velocity_render_multiplier * lastState.vy);


                if (render_clean)
                {
                    frame.Draw(new CircleF(new PointF(x, y), 10), _stateColorGreen, 2);
                    //frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), new Bgr(0.0, 0.0, 255.0), 1); //Render velocity vector
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
