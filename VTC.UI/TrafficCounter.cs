using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using VTC.Kernel;
using VTC.Kernel.Extensions;
using VTC.Kernel.Vistas;
using VTC.ServerReporting.ReportItems;
using VTC.Settings;
using VTC.Kernel.Extensions;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
       private readonly AppSettings _settings;
       private readonly string _filename; // filename with local video (debug mode).
      private static Capture _cameraCapture;

      private DateTime _applicationStartTime;

      private List<CaptureSource> _cameras = new List<CaptureSource>(); //List of all video input devices. Index, file location, name
      private CaptureSource _selectedCamera = null;
      private CaptureSource SelectedCamera
      {
          set
          {
              if (value == _selectedCamera) return;

              _selectedCamera = value;

              if (null != _cameraCapture)
              {
                  _cameraCapture.Dispose();
              }

              _cameraCapture = _selectedCamera.GetCapture();

              _cameraCapture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, _settings.FrameHeight);
              _cameraCapture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, _settings.FrameWidth);

              Vista = new IntersectionVista(_settings, _cameraCapture.Width, _cameraCapture.Height);
          }
      }

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
          InitializeComponent();

           _settings = settings;
           _filename = filename;

          //Initialize the camera selection combobox.
          InitializeCameraSelection();

          //Initialize parameters.
          LoadParameters();

          System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
           t.Interval = 5000;
           t.Tick += TOnTick;
           t.Start();

           _applicationStartTime = DateTime.Now;
           Run();
      }

       private void TOnTick(object sender, EventArgs eventArgs)
       {
           resampleBackgroundButton.Image = null;
           var image = new Emgu.CV.Image<Bgr, Byte>(800, 600);
           Vista.VelocityField.Draw(image, new Bgr(System.Drawing.Color.White), 1);
           resampleBackgroundButton.Image = image;
       }

       void Run()
      {
         try
         {
            SelectedCamera = _cameras.First();
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
            return;
         }
         
         Application.Idle += ProcessFrame;

         String intersectionImagePath = "ftp://"+ _settings.ServerUrl +"/intersection_" + _settings.IntersectionID + ".png";
         ServerReporter.INSTANCE.AddReportItem(
             new FtpSendFileReportItem(
                 _settings.FrameUploadIntervalMinutes, 
                 new Uri(intersectionImagePath),
                  new NetworkCredential(_settings.FtpUserName, _settings.FtpPassword),
                  GetCameraFrameBytes
                 ));

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

       private void AddCamera(CaptureSource camera)
       {
           _cameras.Add(camera);
           CameraComboBox.Items.Add(camera.ToString());
       }

      /// <summary>
      /// Method for initializing the camera selection combobox.
      /// </summary>
      void InitializeCameraSelection()
      {
          // Add video file as source, if provided
          if (UseLocalVideo(_filename))
          {
              AddCamera(new VideoFileCapture("File: " + Path.GetFileName(_filename), _filename));
          }

          //List all video input devices.
          DsDevice[] systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

          //Variable to indicate the device´s index.
          int deviceIndex = 0;

          //Add every device to the global variable and to the camera combobox
          foreach (DsDevice camera in systemCameras)
          {
              //Add Device with an index and a name to the List.
              AddCamera(new SystemCamera(camera.Name, deviceIndex));

              //Increment the index.
              deviceIndex++;
          }

          if (File.Exists("ipCameras.txt"))
          {
              var ipCameraStrings = File.ReadAllLines("ipCameras.txt");

              foreach (var str in ipCameraStrings)
              {
                  var split = str.Split(',');
                  if (split.Count() != 2) continue;

                  AddCamera(new IpCamera(split[0], split[1]));
              }
          }

          //Disable eventhandler for the changing combobox index.
          CameraComboBox.SelectedIndexChanged -= CameraComboBox_SelectedIndexChanged;

          //Set the index if a device could be found.
          if (_cameras.Count > 0)
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
        //Change the capture device.
        SelectedCamera = _cameras[CameraComboBox.SelectedIndex];   
      }

      /// <summary>
      /// Method for refreshing the background.
      /// </summary>
      private void RefreshBackground(Image<Bgr, Byte> frame) 
      {
          //Refresh background.
          Vista.InitializeBackground(frame);
      }
      
       /// <summary>
      /// Method to load user settings.
      /// </summary>
      private void LoadParameters()
      {
          intersectionIDTextBox.Text = _settings.IntersectionID;
          pushStateTimer.Interval = _settings.StateUploadIntervalMs;
          serverUrlTextBox.Text = _settings.ServerUrl;

          frameHeightBox.Text = _settings.FrameHeight.ToString(CultureInfo.InvariantCulture);
          frameWidthBox.Text = _settings.FrameWidth.ToString(CultureInfo.InvariantCulture);
      }

      /// <summary>
      /// Method to save user settings.
      /// </summary>
      private void SaveParametersBtn_Click(object sender, EventArgs e)
      {
          try
          {
              _settings.IntersectionID = intersectionIDTextBox.Text;
              _settings.ServerUrl = serverUrlTextBox.Text;

              _settings.FrameHeight = double.Parse(frameHeightBox.Text);
              _settings.FrameWidth = double.Parse(frameWidthBox.Text);

              _settings.Save();
          }
          catch (Exception ex)
          {
              var message = "Cannot save configuration settings. Error: " + ex.Message;
              Log(message);
              MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          using (Image<Bgr, Byte> frame = _cameraCapture.QueryFrame())
          {
              
              //Workaround - dispose and restart capture if camera starts returning null. 
              //TODO: Investigate - why is this necessary?
              if (frame == null) 
              {
                _cameraCapture.Dispose();
                _cameraCapture = _selectedCamera.GetCapture();
                Debug.WriteLine("Restarting camera: " + DateTime.Now);
                return;
              }

              // Send the new image frame to the vista for processing
              Vista.Update(frame);

              // Update image boxes
              imageBox1.Image = Vista.GetCurrentStateImage();
              imageBox2.Image = Vista.GetBackgroundImage(showPolygonsCheckbox.Checked);
              //resampleBackgroundButton.Image = Vista.Movement_Mask;

              // Update statistics
              trackCountBox.Text = Vista.CurrentVehicles.Count.ToString();

              tbVistaStats.Text = Vista.GetStatString();

              //System.Threading.Thread.Sleep(33);
              TimeSpan activeTime = (DateTime.Now - _applicationStartTime);
              timeActiveTextBox.Text = activeTime.ToString(@"dd\.hh\:mm\:ss");

          }
      }

      void PushStateProcess(object sender, EventArgs e)
      {
        if (pushStateCheckbox.Checked)
        {
            Thread pushThread = new Thread(PushState);
            pushThread.Start();
        }
      }

      private void PushState()
      {
          bool success = false;
          try
          {
              StateEstimate[] stateEstimates = Vista.CurrentVehicles.Select(v => v.state_history.Last()).ToArray();
              string postString;
              string postUrl = ServerReporting.ReportItems.HttpPostReportItem.PostStateString(stateEstimates, _settings.IntersectionID, _settings.ServerUrl, out postString);
              ServerReporting.ReportItems.HttpPostReportItem.SendStatePOST(postUrl, postString);
              success = true;
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
          finally
          {
              Debug.WriteLine("Post state success: " + success + " " + DateTime.Now);
          }
      }

       

       private void imageBox1_MouseDown(object sender, MouseEventArgs e)
      {
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

      private void resampleBackgroundButton_Click(object sender, EventArgs e)
      {
          Image<Bgr, Byte> frame = _cameraCapture.QueryFrame();
          if(frame != null)
            RefreshBackground(frame);
      }
   }
}