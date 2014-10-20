using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using DirectShowLib;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using TreeLib;
using VTC.ServerReporting.ReportItems;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
      private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
      private static Capture _cameraCapture;

      List<KeyValuePair<int, string>> _cameraDevices = new List<KeyValuePair<int, string>>();   //List of all video input devices. 
      bool _changeInFileMode = true;        //Boolean variable indicating, if the user can choose a webcam, while he was
                                            //using a prerecorded video.
      
      //************* Main image variables ***************
      Image<Bgr, Byte> Frame;               //current Frame from camera
      Image<Bgr, float> Color_Background;   //Average Background being formed
      Image<Bgr, float> Color_Difference;   //Difference between the two frames
      Image<Gray, byte> Movement_Mask;      //Thresholded, b&w movement mask
      Image<Bgr, float> ROI_image;          //Area occupied by traffic

      //************* Background subtraction parameters ***************
      double alpha = Convert.ToDouble(ConfigurationManager.AppSettings["Alpha"]);                   //stores alpha for thread access
      int color_threshold = Convert.ToInt32(ConfigurationManager.AppSettings["ColorThreshold"]);    //Threshold below which frame-movement is ignored

      //************* Object detection parameters ***************  
      int car_radius = Convert.ToInt32(ConfigurationManager.AppSettings["CarRadius"]);              //Radius of car image in pixels
      double noise_mass = Convert.ToDouble(ConfigurationManager.AppSettings["NoiseMass"]);          //Background movement noise
      double per_car = Convert.ToDouble(ConfigurationManager.AppSettings["PerCar"]);                //White pixels per car in image
      double per_car_minimum = Convert.ToDouble(ConfigurationManager.AppSettings["PerCarMin"]);     //Minimum number of white pixels per car - handles case when 0 is entered in avg-per-car textbox
      int max_object_count = Convert.ToInt32(ConfigurationManager.AppSettings["MaxObjCount"]);      //Maximum number of blobs to detect
      private RegionConfig _regionConfig;                                                           //Used to select ROI polygons

      //************* Multiple hypothesis tracking parameters ***************  
      private MultipleHypothesisTracker MHT = null;
      double pruning_ratio = Convert.ToDouble(ConfigurationManager.AppSettings["PruningRatio"]);    //Probability ratio at which hypotheses are pruned
      double q = Convert.ToDouble(ConfigurationManager.AppSettings["Q"]);                           //Process noise matrix multiplier
      double r = Convert.ToDouble(ConfigurationManager.AppSettings["R"]);                           //Measurement noise matrix multiplier

      //************* Rendering parameters ***************  
      double velocity_render_multiplier = 1.0; //Velocity is multiplied by this quantity to give a line length for rendering
      bool render_clean = true;                //Don't draw velocity vector, use fixed-size object circles. Should add this as checkbox to UI.

      bool VIDEO_FILE = false;

      //************* Server connection parameters ***************  
      string intersection_id = ConfigurationManager.AppSettings["IntersectionId"];
      private int FRAME_UPLOAD_INTERVAL_MINUTES =Convert.ToInt16(ConfigurationManager.AppSettings["FRAME_UPLOAD_INTERVAL_MINUTES"]);
      string ftp_password = ConfigurationManager.AppSettings["FTPpassword"];
      string ftp_username = ConfigurationManager.AppSettings["FTPusername"];
      private int state_upload_interval_ms = Convert.ToInt32(ConfigurationManager.AppSettings["state_upload_interval_ms"]);
       
      public TrafficCounter()
      {
         Trace.WriteLine("Initializing VTC...");
         MHT = new MultipleHypothesisTracker();

         InitializeComponent();

         //Initialize the camera selection combobox.
         InitializeCameraSelection();

         //Initialize parameters.
         LoadParameters();
         Run();
         Trace.WriteLine("Initializing finished.");
      }

      public TrafficCounter(string argument)
      {
          Trace.WriteLine("Initializing VTC with canned video...");
          MHT = new MultipleHypothesisTracker();

          if (argument == "VIDEO_FILE")
              VIDEO_FILE = true;
      
          InitializeComponent();

          //Initialize the camera selection combobox.
          InitializeCameraSelection();

          //Initialize parameters.
          LoadParameters();

          Run();
          Trace.WriteLine("Initializing finished.");
      }

		private RegionConfig RegionConfig
		{
			get
			{
				return _regionConfig;
			}
			set
			{
				_regionConfig = value;
				
				ROI_image = RegionConfig.RoiMask.GetMask(_cameraCapture.Width, _cameraCapture.Height, new Bgr(Color.White));
			}
		}

      void Run()
      {
         try
         {
             if(VIDEO_FILE==false)
             {
                _cameraCapture = new Capture(0);
             }
             else 
             {
                 _cameraCapture = new Capture(ConfigurationManager.AppSettings["VideoFilePath"]);
             }

             RegionConfig = RegionConfig.Load(ConfigurationManager.AppSettings["RegionConfig"]);
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
            return;
         }
         
         Application.Idle += ProcessFrame;

          //TODO: Move server credentials to configuration file
         String IntersectionImagePath = "ftp://www.traffic-camera.com/intersection_" + intersection_id + ".png";
         ServerReporter.INSTANCE.AddReportItem(
             new FtpSendFileReportItem(
                 FRAME_UPLOAD_INTERVAL_MINUTES,
                 new Uri(@IntersectionImagePath),
                  new NetworkCredential(ftp_username, ftp_password),
                  GetCameraFrameBytes
                 ));
      }

      /// <summary>
      /// Method for initializing the camera selection combobox.
      /// </summary>
      void InitializeCameraSelection()
      {
          //List all video input devices.
          DsDevice[] _systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

          //Variable to indicate the device´s index.
          int _deviceIndex = 0;

          //Add every device to the global variable and to the camera combobox
          foreach (DirectShowLib.DsDevice _camera in _systemCameras)
          {
              //Add Device with an index and a name to the List.
              _cameraDevices.Add(new KeyValuePair<int, string>(_deviceIndex, _camera.Name));

              //Add a combobox item.
              CameraComboBox.Items.Add(_camera.Name);

              //Increment the index.
              _deviceIndex++;
          }

          //Disable eventhandler for the changing combobox index.
          CameraComboBox.SelectedIndexChanged -= CameraComboBox_SelectedIndexChanged;

          //Set the index if a device could be found.
          if (_cameraDevices.Count > 0)
          {
              CameraComboBox.SelectedIndex = 0;
          }

          //Enable eventhandler for the changing combobox index.
          CameraComboBox.SelectedIndexChanged += CameraComboBox_SelectedIndexChanged;
      }

      /// <summary>
      /// Method which reacts to the change of the camera selection combobox.
      /// </summary>
      private void CameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
          //Continue if it´s allowed to select a camera during the use of an prerecorded video.
          if (_changeInFileMode == true)
          {
              //Change the capture device.
              _cameraCapture = new Capture(CameraComboBox.SelectedIndex);

              //TODO: Reset processing variables.
              //...
          }
      }
      
       /// <summary>
      /// Method to load user settings.
      /// </summary>
      private void LoadParameters()
      {
          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          configPathBox.Text = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

          avgAreaTextbox.Text = config.AppSettings.Settings["PerCar"].Value;
          avgNoiseTextbox.Text = config.AppSettings.Settings["PerCarMin"].Value;

          intersectionIDTextBox.Text = intersection_id.ToString();
          pushStateTimer.Interval = state_upload_interval_ms;
      }

      /// <summary>
      /// Method to save user settings.
      /// </summary>
      private void SaveParametersBtn_Click(object sender, EventArgs e)
      {
          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          
          config.AppSettings.Settings["PerCar"].Value = avgAreaTextbox.Text;
          config.AppSettings.Settings["PerCarMin"].Value = avgNoiseTextbox.Text;
          config.AppSettings.Settings["IntersectionID"].Value = intersectionIDTextBox.Text;

          config.Save(ConfigurationSaveMode.Modified);

          ConfigurationManager.RefreshSection("appSettings");
      }

      private Byte[] GetCameraFrameBytes()
      {
          // TODO:  Want PNG?

          var bmp = Color_Background.ToBitmap();

          byte[] byteArray;
          using (MemoryStream stream = new MemoryStream())
          {
              bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
              stream.Close();

              byteArray = stream.ToArray();
          }

          return byteArray;
      }

      void ProcessFrame(object sender, EventArgs e)
      {
          Image<Bgr, Byte> frame = _cameraCapture.QueryFrame();

          if (frame == null)
              return;

          Frame = frame;

          if (Color_Background == null) //we need at least one frame to initialize the background
          {
              Color_Background = frame.Convert<Bgr, float>();
          }
          else
          {
              int count = CountObjects();
              if (count > max_object_count)
                  count = max_object_count;

              UpdateBackground(frame);

              Coordinates[] coordinates = FindBlobCenters(frame, count);

              if (null != MHT)
                MHT.Update(coordinates);
          }

          renderUI(frame);

      }

      void PushStateProcess(object sender, EventArgs e)
      {
        if (pushStateCheckbox.Checked)
        {
            Thread pushThread = new Thread(pushState);
            pushThread.Start();
        }
      }

      private static void CoordinatesContains(Coordinates[] coordinates, double problem_x, double problem_y)
      {
          for (int i = 0; i < coordinates.Length; i++)
          {
              if (coordinates[i].x == problem_x && coordinates[i].y == problem_y)
                  Console.WriteLine("Multiple detections here");
          }
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
          noise_mass = Convert.ToInt32(avgNoiseTextbox.Text);
          per_car = Convert.ToInt32(avgAreaTextbox.Text);
          if (per_car <= per_car_minimum)
              per_car = per_car_minimum;

          int count = Convert.ToInt32((raw_mass - noise_mass) / per_car);
          if (count < 0)
              count = 0;
          if (count > max_object_count)
              count = max_object_count;

          movementMassBox.Text = raw_mass.ToString();
          detectionCountBox.Text = count.ToString();
          return count;
      }

      private void pushState()
      {
          try
          {

              StateEstimate[] state_estimates = MHT.Vehicles.Select(v => v.state_history.Last()).ToArray();
              Dictionary<string, string> post_values = new Dictionary<string, string>();
              for (int vehicle_count = 0; vehicle_count < state_estimates.Length; vehicle_count++)
              {
                  String x = state_estimates[vehicle_count].coordinates.x.ToString();
                  String y = state_estimates[vehicle_count].coordinates.y.ToString();
                  String vx = state_estimates[vehicle_count].vx.ToString();
                  String vy = state_estimates[vehicle_count].vy.ToString();
                  String zero = "0";
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][x]", x);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][vx]", vx);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][y]", y);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][vy]", vy);
                  post_values.Add("state_sample[states_attributes][" + vehicle_count.ToString() + "][_destroy]", zero);
              }
              
              if(state_estimates.Length == 0)
                  post_values.Add("state_sample[states_attributes][]", "");

              post_values.Add("intersection_id", intersection_id);


              String post_string = "";
              foreach (KeyValuePair<string, string> post_value in post_values)
              {
                  post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
              }
              post_string = post_string.TrimEnd('&');

              //Upload state to server
              String post_url = "http://www.traffic-camera.com/state_samples";

              HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
              objRequest.KeepAlive = true;
              objRequest.Pipelined = true;
              objRequest.Timeout = 2000;
              objRequest.Method = "POST";
              objRequest.ContentLength = post_string.Length;
              objRequest.ContentType = "application/x-www-form-urlencoded";

              //// post data is sent as a stream
              StreamWriter myWriter = null;
              myWriter = new StreamWriter(objRequest.GetRequestStream());
              myWriter.Write(post_string);
              myWriter.Close();
              objRequest.GetResponse();
          }
          catch (Exception ex)
          {
            #if(DEBUG)
            {
                Console.WriteLine(ex.Message);
                throw (ex);
            }
            #else
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.InnerException);
                Trace.WriteLine(ex.StackTrace);
                Trace.WriteLine(ex.TargetSite);
            }
            #endif
          }
      }

      private void renderUI(Image<Bgr, Byte> frame)
      {
          Image<Bgr, float> Overlay = new Image<Bgr, float>(ROI_image.Width, ROI_image.Height, new Bgr(Color.Green));
          Overlay = Overlay.And(ROI_image);
          Overlay.Acc(Color_Background);

          if (null == MHT) 
              return;

          var vehicles = MHT.Vehicles;

          vehicles.ForEach(delegate(Vehicle vehicle)
          {
              float x = (float) vehicle.state_history.Last().coordinates.x;
              float y = (float) vehicle.state_history.Last().coordinates.y;

              var validation_region_deviation = MHT.ValidationRegionDeviation;

              float radius = validation_region_deviation*((float)Math.Sqrt(Math.Pow(vehicle.state_history.Last().cov_x,2) + (float) Math.Pow(vehicle.state_history.Last().cov_y,2)));
              if (radius < 2.0)
                  radius = (float) 2.0;

              float vx_render = (float) (velocity_render_multiplier * vehicle.state_history.Last().vx);
              float vy_render = (float) (velocity_render_multiplier * vehicle.state_history.Last().vy);

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

          trackCountBox.Text = vehicles.Count().ToString();

          imageBox1.Image = frame;
          if (showPolygonsCheckbox.Checked)
              imageBox2.Image = Overlay;
          else
            imageBox2.Image = Color_Background;
          
          imageBox3.Image = Movement_Mask;
          
      }


      private void imageBox1_MouseDown(object sender, MouseEventArgs e)
      {
          int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
          int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
          int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
          int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
      }

      private void btnConfigureRegions_Click(object sender, EventArgs e)
      {
          RegionEditor r = new RegionEditor(Color_Background, RegionConfig);
          if (r.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
              RegionConfig = r.GetRegionConfig();
              RegionConfig.Save(ConfigurationManager.AppSettings["RegionConfig"]);
          }
      }

      private void TrafficCounter_FormClosed(object sender, FormClosedEventArgs e)
      {
          Application.Idle -= ProcessFrame;
      }

      private void pushStateCheckbox_CheckedChanged(object sender, EventArgs e)
      {
          if (pushStateCheckbox.Checked)
          {
              ServerReporter.INSTANCE.Start();
          }
          else
          {
              ServerReporter.INSTANCE.Stop();
          }
      }
   }
}