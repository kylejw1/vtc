using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using TreeLib;
using VTC.ServerReporting.ReportItems;
using VTC.Settings;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
      private static MCvFont _font = new MCvFont(FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
       private readonly AppSettings _settings;
       private readonly string _filename; // filename with local video (debug mode).
      private static Capture _cameraCapture;

      List<KeyValuePair<int, string>> _cameraDevices = new List<KeyValuePair<int, string>>();   //List of all video input devices. 
      bool _changeInFileMode = true;        //Boolean variable indicating, if the user can choose a webcam, while he was
                                            //using a prerecorded video.

      //************* Multiple hypothesis tracking parameters ***************  
      Vista Vista = null;
       
       //private MultipleHypothesisTracker MHT = null;

       /// <summary>
       /// Constructor.
       /// </summary>
       /// <param name="settings">Application settings.</param>
       /// <param name="filename">Local file with video.</param>
       public TrafficCounter(AppSettings settings, string filename = null)
      {
           _settings = settings;
           _filename = filename;

          InitializeComponent();

          //Initialize the camera selection combobox.
          InitializeCameraSelection();

          //Initialize parameters.
          LoadParameters();

          Run();
      }

      void Run()
      {
         try
         {
             if (UseLocalVideo(_filename))
             {
                 _cameraCapture = new Capture(_filename);
             }
             else
             {
                 _cameraCapture = new Capture(0);
             }

             Vista = new IntersectionVista(_settings, _cameraCapture.Width, _cameraCapture.Height);

             var regionConfig = RegionConfig.Load(_settings.RegionConfigPath);
             if (null != regionConfig) Vista.RegionConfiguration = regionConfig;
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
            return;
         }
         
         Application.Idle += ProcessFrame;

          //TODO: Move server credentials to configuration file
         String IntersectionImagePath = "ftp://"+ _settings.ServerUrl +"/intersection_" + _settings.IntersectionID + ".png";
         ServerReporter.INSTANCE.AddReportItem(
             new FtpSendFileReportItem(
                 _settings.FrameUploadIntervalMinutes, 
                 new Uri(@IntersectionImagePath),
                  new NetworkCredential(_settings.FtpUserName, _settings.FtpPassword),
                  GetCameraFrameBytes
                 ));
         ServerReporter.INSTANCE.Start();
      }

       /// <summary>
       /// Write log message.
       /// </summary>
       /// <param name="format">Message format.</param>
       /// <param name="args">Format argument.</param>
       private static void Log(string format, params object[] args)
       {
           Console.WriteLine(format, args);
       }

       /// <summary>
       /// Check if video file exists.
       /// </summary>
       /// <param name="filename">Pathname to check.</param>
       /// <returns><c>true</c> for existing file.</returns>
       private static bool UseLocalVideo(string filename)
       {
           if (string.IsNullOrWhiteSpace(filename)) return false;
           if (File.Exists(filename)) return true;

           Log("Video file is not found ('{0}').", filename);

           return false;
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
          foreach (DsDevice _camera in _systemCameras)
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

          avgAreaTextbox.Text = _settings.PerCar.ToString(CultureInfo.InvariantCulture); 
          avgNoiseTextbox.Text = _settings.PerCarMinimum.ToString(CultureInfo.InvariantCulture);

          intersectionIDTextBox.Text = _settings.IntersectionID;
          pushStateTimer.Interval = _settings.StateUploadIntervalMs;
      }

      /// <summary>
      /// Method to save user settings.
      /// </summary>
      private void SaveParametersBtn_Click(object sender, EventArgs e)
      {
          try
          {
              _settings.PerCar = double.Parse(avgAreaTextbox.Text, CultureInfo.InvariantCulture);
              _settings.PerCarMinimum = double.Parse(avgNoiseTextbox.Text, CultureInfo.InvariantCulture);
              _settings.IntersectionID = intersectionIDTextBox.Text;

              _settings.Save();
          }
          catch (Exception ex)
          {
              var message = "Cannot save configuration settings. Error: " + ex.Message;
              Log(message);
              MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
      }

       /// <summary>
       /// Get the current frame in the form of a PNG byte array
       /// </summary>
       /// <returns>Byte array containing the current frame in PNG format</returns>
      private Byte[] GetCameraFrameBytes()
      {
           using (var bmp = Vista.Color_Background.ToBitmap())
           using (var stream = new MemoryStream())
           {
               bmp.Save(stream, ImageFormat.Png);
               stream.Close();

               return stream.ToArray();
           }
      }

      void ProcessFrame(object sender, EventArgs e)
      {
          Image<Bgr, Byte> frame = _cameraCapture.QueryFrame();

          if (frame == null)
              return;

          // Send options text box data to the Vista
          // TODO:  'Vista' class has a per_car_minimum that should be set in the text box if the 
          // user specifies a value that is too low.
          // Also, do we want an "apply" button?  I feel like if the frame updates while a user has helf-changed the 
          // per_car value, we could get some bad values?
          Vista.NoiseMass = Convert.ToInt32(avgNoiseTextbox.Text);
          Vista.PerCar = Convert.ToInt32(avgAreaTextbox.Text);

          // Send the new image frame to the vista for processing
          Vista.Update(frame);

          // Update image boxes
          imageBox1.Image = Vista.GetCurrentStateImage();
          imageBox2.Image = Vista.GetBackgroundImage(showPolygonsCheckbox.Checked);
          imageBox3.Image = Vista.Movement_Mask;

          // Update statistics
          trackCountBox.Text = Vista.CurrentVehicles.Count().ToString();
          movementMassBox.Text = Vista.RawMass.ToString();
          detectionCountBox.Text = Vista.LastDetectionCount.ToString();

          tbVistaStats.Text = Vista.GetStatString();
      }

      void PushStateProcess(object sender, EventArgs e)
      {
        if (pushStateCheckbox.Checked)
        {
            Thread pushThread = new Thread(pushState);
            pushThread.Start();
        }
      }

      private static void CoordinatesContains(Measurements[] coordinates, double problem_x, double problem_y)
      {
          for (int i = 0; i < coordinates.Length; i++)
          {
              if (coordinates[i].x == problem_x && coordinates[i].y == problem_y)
                  Console.WriteLine("Multiple detections here");
          }
      }

      private void pushState()
      {
          try
          {

              StateEstimate[] state_estimates = Vista.CurrentVehicles.Select(v => v.state_history.Last()).ToArray();
              Dictionary<string, string> post_values = new Dictionary<string, string>();
              for (int vehicle_count = 0; vehicle_count < state_estimates.Length; vehicle_count++)
              {
                  String x = state_estimates[vehicle_count].x.ToString();
                  String y = state_estimates[vehicle_count].y.ToString();
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

              post_values.Add("intersection_id", _settings.IntersectionID);


              String post_string = "";
              foreach (KeyValuePair<string, string> post_value in post_values)
              {
                  post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
              }
              post_string = post_string.TrimEnd('&');

              //Upload state to server
              String post_url = "http://" + _settings.ServerUrl + "/state_samples";

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
                //throw (ex);
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

      private void imageBox1_MouseDown(object sender, MouseEventArgs e)
      {
          int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
          int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
          int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
          int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
      }

      private void btnConfigureRegions_Click(object sender, EventArgs e)
      {
          RegionEditor r = new RegionEditor(Vista.Color_Background, Vista.RegionConfiguration);
          if (r.ShowDialog() == DialogResult.OK)
          {
              Vista.RegionConfiguration = r.RegionConfig;
              Vista.RegionConfiguration.Save(_settings.RegionConfigPath);
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