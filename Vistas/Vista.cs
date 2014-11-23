using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using TreeLib;

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
        //************* Abstract methods ****************
        protected abstract void UpdateChildClassStats(List<Vehicle> deleted);

        //************* Main image variables ***************
        private Image<Bgr, Byte> Frame;               //current Frame from camera
        private Image<Bgr, float> Color_Difference;   //Difference between the two frames
        private Image<Bgr, float> ROI_image;          //Area occupied by traffic
        private int Width;
        private int Height;
        public Image<Gray, byte> Movement_Mask { get; private set; }      //Thresholded, b&w movement mask
        public Image<Bgr, float> Color_Background { get; private set; }   //Average Background being formed

        //************* Background subtraction parameters ***************
        private double alpha = Convert.ToDouble(ConfigurationManager.AppSettings["Alpha"]);                   //stores alpha for thread access
        private int color_threshold = Convert.ToInt32(ConfigurationManager.AppSettings["ColorThreshold"]);    //Threshold below which frame-movement is ignored

        //************* Object detection parameters ***************  
        private int car_radius = Convert.ToInt32(ConfigurationManager.AppSettings["CarRadius"]);              //Radius of car image in pixels
        private double per_car_minimum = Convert.ToDouble(ConfigurationManager.AppSettings["PerCarMin"]);     //Minimum number of white pixels per car - handles case when 0 is entered in avg-per-car textbox
        private int max_object_count = Convert.ToInt32(ConfigurationManager.AppSettings["MaxObjCount"]);      //Maximum number of blobs to detect
        public double per_car = Convert.ToDouble(ConfigurationManager.AppSettings["PerCar"]);                //White pixels per car in image
        public double noise_mass = Convert.ToDouble(ConfigurationManager.AppSettings["NoiseMass"]);          //Background movement noise
        public double RawMass { get; private set; }
        public int LastDetectionCount { get; private set; }

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

                ROI_image = RegionConfiguration.RoiMask.GetMask(Width, Height, new Bgr(Color.White));

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

                ROI_image = RegionConfiguration.RoiMask.GetMask(Width, Height, new Bgr(Color.White));
            }
        }

        public List<Vehicle> CurrentVehicles
        {
            get { return MHT.CurrentVehicles; }
        }

        protected Vista(int width, int height)
        {
            Width = width;
            Height = height;
            MHT = new MultipleHypothesisTracker();
            RegionConfiguration = new RegionConfig();
            EventConfiguration = new EventConfig();

            LastDetectionCount = 0;
            RawMass = 0;
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

                if (count > max_object_count)
                    count = max_object_count;

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
            Image<Bgr, float> BackgroundUpdate = frame.Convert<Bgr, float>();
            Color_Background.RunningAvg(BackgroundUpdate, alpha);
        }

        private int CountObjects()
        {
            Color_Difference = Color_Background.AbsDiff(Frame.Convert<Bgr, float>());
            Image<Bgr, float> Masked_Difference = Color_Difference.And(ROI_image);
            Masked_Difference._ThresholdBinary(new Bgr(color_threshold, color_threshold, color_threshold), new Bgr(Color.White));
            Movement_Mask = Masked_Difference.Convert<Gray, Byte>();
            Movement_Mask._SmoothGaussian(5, 5, 1, 1);
            Image<Bgr, double> int_img = Masked_Difference.Integral();
            int lim_x = Masked_Difference.Width - 1;
            int lim_y = Masked_Difference.Height - 1;
            double raw_mass = (int_img[lim_y, lim_x].Blue + int_img[lim_y, lim_x].Red + int_img[lim_y, lim_x].Green);
            
            if (per_car <= per_car_minimum)
                per_car = per_car_minimum;

            int count = Convert.ToInt32((raw_mass - noise_mass) / per_car);
            if (count < 0)
                count = 0;
            if (count > max_object_count)
                count = max_object_count;

            return count;
        }

        private Coordinates[] FindBlobCenters(Image<Bgr, Byte> frame, int count)
        {
            Image<Gray, Byte> tempMovement_Mask = Movement_Mask.Clone();
            Coordinates[] coordinates = new Coordinates[count];
            for (int detection_count = 0; detection_count < count; detection_count++)
            {
                double[] minValues;
                double[] maxValues;
                System.Drawing.Point[] minLocations;
                System.Drawing.Point[] maxLocations;
                tempMovement_Mask.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                int[] maxLocation = new int[] { maxLocations[0].X, maxLocations[0].Y };
                tempMovement_Mask.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), car_radius), new Gray(0), 0);
                frame.Draw(new CircleF(new PointF(maxLocation[0], maxLocation[1]), 1), new Bgr(255.0, 255.0, 255.0), 1);
                coordinates[detection_count].x = maxLocation[0];
                coordinates[detection_count].y = maxLocation[1];
            }
            return coordinates;
        }

        public Image<Bgr, float> GetBackgroundImage(bool drawMaskPolygons)
        {
            if (drawMaskPolygons)
            {
                Image<Bgr, float> Overlay = new Image<Bgr, float>(Width, Height, new Bgr(Color.Green));
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
                float x = (float)vehicle.state_history.Last().coordinates.x;
                float y = (float)vehicle.state_history.Last().coordinates.y;

                var validation_region_deviation = MHT.ValidationRegionDeviation;

                float radius = validation_region_deviation * ((float)Math.Sqrt(Math.Pow(vehicle.state_history.Last().cov_x, 2) + (float)Math.Pow(vehicle.state_history.Last().cov_y, 2)));
                if (radius < 2.0)
                    radius = (float)2.0;

                float vx_render = (float)(velocity_render_multiplier * vehicle.state_history.Last().vx);
                float vy_render = (float)(velocity_render_multiplier * vehicle.state_history.Last().vy);


                if (render_clean)
                {
                    frame.Draw(new CircleF(new PointF(x, y), 10), new Bgr(0.0, 255.0, 0.0), 2);
                    //frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), new Bgr(0.0, 0.0, 255.0), 1); //Render velocity vector
                }
                else
                {
                    frame.Draw(new CircleF(new PointF(x, y), radius), new Bgr(0.0, 255.0, 0.0), 1);
                    frame.Draw(new LineSegment2D(new Point((int)x, (int)y), new Point((int)(x + vx_render), (int)(y + vy_render))), new Bgr(0.0, 0.0, 255.0), 1);
                }

            }
           );

            return frame;
        }
    }
}
