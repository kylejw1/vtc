using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;
using VTC.Kernel;
using VTC.Kernel.Vistas;
using VTC.Reporting;
using VTC.Reporting.ReportItems;
using VTC.Settings;
using VTC.ExportTrainingSet;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
      private readonly AppSettings _settings;
      private readonly string _filename; // filename with local video (debug mode).
       private const string IpCamerasFilename = "ipCameras.txt";

      private readonly DateTime _applicationStartTime;

      private readonly List<CaptureSource> _cameras = new List<CaptureSource>(); //List of all video input devices. Index, file location, name
      private CaptureSource _selectedCamera;

       /// <summary>
       /// Active camera.
       /// </summary>
      private CaptureSource SelectedCamera
      {
          get { return _selectedCamera; }
          set
          {
              if (value == _selectedCamera) return;

              if (_selectedCamera != null)
              {
                  _selectedCamera.Destroy();
              }

              _selectedCamera = value;
              _selectedCamera.Init(_settings);

              _vista = new IntersectionVista(_settings, _selectedCamera.Width, _selectedCamera.Height);
          }
      }

      //************* Multiple hypothesis tracking parameters ***************  
      Vista _vista = null;
       
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

           //Clear sample files
           ClearRGBSamplesFile();

           _applicationStartTime = DateTime.Now;
          Run();
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

          if (File.Exists(IpCamerasFilename))
          {
              var ipCameraStrings = File.ReadAllLines(IpCamerasFilename);

              foreach (var str in ipCameraStrings)
              {
                  var split = str.Split(',');
                  if (split.Count() != 2) continue;

                  AddCamera(new IpCamera(split[0], split[1]));
              }
          }

          PopulateUnitTests();

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

       private void PopulateUnitTests()
       {
           
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
          _vista.InitializeBackground(frame);
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
           using (var bmp = _vista.Color_Background.ToBitmap())
           using (var stream = new MemoryStream())
           {
               bmp.Save(stream, ImageFormat.Png);
               stream.Close();

               return stream.ToArray();
           }
      }

      void ProcessFrame(object sender, EventArgs e)
      {
          using (Image<Bgr, Byte> frame = SelectedCamera.QueryFrame())
          {
              
              //Workaround - dispose and restart capture if camera starts returning null. 
              //TODO: Investigate - why is this necessary?
              if (frame == null)
              {
                  SelectedCamera.Destroy();
                  SelectedCamera.Init(_settings);
                  
                Debug.WriteLine("Restarting camera: " + DateTime.Now);
                return;
              }

              Image<Bgr, Byte> frameForProcessing = frame.Clone(); // Necessary so that frame.Data becomes accessible

              // Send the new image frame to the vista for processing
              _vista.Update(frameForProcessing);

              // Update image boxes
              UpdateImageBoxes(frameForProcessing);

              // Update statistics
              trackCountBox.Text = _vista.CurrentVehicles.Count.ToString();
              tbVistaStats.Text = _vista.GetStatString();

              // Save R,G,B samples
              StoreRGBSample(frameForProcessing);

              //System.Threading.Thread.Sleep(33);
              TimeSpan activeTime = (DateTime.Now - _applicationStartTime);
              timeActiveTextBox.Text = activeTime.ToString(@"dd\.hh\:mm\:ss");

          }
      }


       private void ClearRGBSamplesFile()
       {
           string path = @"RGB.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
            }
       }

       private void StoreRGBSample(Image<Bgr, byte> frame)
       {
           if (frame != null && saveRGBSamplesCheckBox.Checked)
           {
               Bgr pixel = frame[RGBSamplePoint.Y, RGBSamplePoint.X];
               double r = pixel.Red;
               double g = pixel.Green;
               double b = pixel.Blue;

               string path = @"RGB.txt";
               using (StreamWriter sw = File.AppendText(path))
                   sw.WriteLine(r + "," + g + "," + b);    
           }
       }

       private void UpdateImageBoxes(Image<Bgr, Byte> frame)
      {
          imageBox1.Image = _vista.GetCurrentStateImage();
          imageBox2.Image = _vista.GetBackgroundImage(showPolygonsCheckbox.Checked);
          imageBox3.Image = _vista.BackgroundUpdateMoG;
          if (_vista.Movement_Mask != null)
          {
              Image<Bgr, Byte> movementTimesImage = frame.And(_vista.Movement_Mask.Convert<Bgr, byte>());
              movementMaskBox.Image = movementTimesImage;
              //resampleBackgroundButton.Image = _vista.Movement_Mask;
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
              StateEstimate[] stateEstimates = _vista.CurrentVehicles.Select(v => v.state_history.Last()).ToArray();
              string postString;
              string postUrl = HttpPostReportItem.PostStateString(stateEstimates, _settings.IntersectionID, _settings.ServerUrl, out postString);
              HttpPostReportItem.SendStatePOST(postUrl, postString);
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
          RegionEditor r = new RegionEditor(_vista.Color_Background, _vista.RegionConfiguration);
          if (r.ShowDialog() == DialogResult.OK)
          {
              _vista.RegionConfiguration = r.RegionConfig;
              _vista.RegionConfiguration.Save(_settings.RegionConfigPath);
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
          using (Image<Bgr, Byte> frame = SelectedCamera.QueryFrame())
          {
              if (frame != null)
                  RefreshBackground(frame);
          }
      }

      private void exportTrainingImagesButton_Click(object sender, EventArgs e)
      {
          using (Image<Bgr, Byte> frame = SelectedCamera.QueryFrame())
          {
              if (frame != null)
              {
                  
                  //ExportTrainingSet.ExportTrainingSet eT = new ExportTrainingSet.ExportTrainingSet(_settings, frame.Convert<Bgr,float>());
                  ExportTrainingSet.ExportTrainingSet eT = new ExportTrainingSet.ExportTrainingSet(_settings, _vista.Training_Image.Convert<Bgr,float>(), _vista.CurrentVehicles);
                  eT.Show();
              }
          }

      }

       private Point RGBSamplePoint;

       private void imageBox1_Click(object sender, EventArgs e)
       {
          if(Cursor.Position.X < imageBox1.Width && Cursor.Position.Y < imageBox1.Height && Cursor.Position.X >= 0 && Cursor.Position.Y >= 0)
          {
              RGBSamplePoint = imageBox1.PointToClient(Cursor.Position);
              rgbCoordinateTextbox.Text = RGBSamplePoint.ToString();
          }
      }
   }
}