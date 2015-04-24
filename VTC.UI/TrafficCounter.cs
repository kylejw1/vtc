using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;
using VTC.CaptureSource;
using VTC.Kernel;
using VTC.Kernel.Extensions;
using VTC.Kernel.RegionConfig;
using VTC.Kernel.Video;
using VTC.Kernel.Vistas;
using VTC.Reporting;
using VTC.Reporting.ReportItems;
using VTC.Settings;
using VTC.Kernel.Extensions;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
      private readonly AppSettings _settings;
       private const string IpCamerasFilename = "ipCameras.txt";

       private VideoDisplay _mainDisplay;
       private VideoDisplay _movementDisplay;
       private VideoDisplay _backgroundDisplay;
       //private VideoDisplay _mixtureDisplay;
       //private VideoDisplay _mixtureMovementDisplay;

      private readonly DateTime _applicationStartTime;

      private readonly List<ICaptureSource> _cameras = new List<ICaptureSource>(); //List of all video input devices. Index, file location, name
      private ICaptureSource _selectedCamera;

        // unit tests has own settings, so need to store "pairs" (capture, settings)
      private CaptureContext[] _testCaptureContexts;


       /// <summary>
       /// Active camera.
       /// </summary>
      private ICaptureSource SelectedCamera
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


              // kind-of ugly... to refactor someday
              if (_unitTestsMode)
              {
                  // create mask for the whole image
                  var polygon = new Polygon();
                  polygon.AddRange(new[]
                    {
                        new Point(0, 0), 
                        new Point(0, (int) _settings.FrameHeight),
                        new Point((int) _settings.FrameWidth, (int) _settings.FrameHeight), 
                        new Point((int) _settings.FrameWidth, 0),
                        new Point(0, 0)
                    });

                  var regionConfig = new RegionConfig
                  {
                      RoiMask = polygon
                  };
                  _vista.RegionConfiguration = regionConfig;
              }
          }
      }

      //************* Multiple hypothesis tracking parameters ***************  
      Vista _vista = null;
       
       //private MultipleHypothesisTracker MHT = null;

       /// <summary>
       /// Constructor.
       /// </summary>
       /// <param name="settings">Application settings.</param>
       /// <param name="appArgument">Can mean different things (Local file with video, Unit tests, etc).</param>
       public TrafficCounter(AppSettings settings, string appArgument = null)
      {
          InitializeComponent();

           _settings = settings;

           // check if app should run in unit test visualization mode
           _unitTestsMode = false;
           if ((appArgument != null) && appArgument.EndsWith(".dll", true, CultureInfo.InvariantCulture))
           {
               _unitTestsMode = DetectTestScenarios(appArgument);
           }

           // otherwise - run in standard mode
           if (! _unitTestsMode)
           {
               InitializeCameraSelection(appArgument);
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

          //Initialize parameters.
          LoadParameters();

          System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
           t.Interval = 5000;
           t.Tick += TOnTick;
           t.Start();
           //Clear sample files
           ClearRGBSamplesFile();

           //Create video windows
           CreateVideoWindows();

           _applicationStartTime = DateTime.Now;
           Run();
      }


       private void TOnTick(object sender, EventArgs eventArgs)
       {
           //resampleBackgroundButton.Image = null;
           //var image = new Emgu.CV.Image<Bgr, Byte>(800, 600);
           //Vista.VelocityField.Draw(image, new Bgr(System.Drawing.Color.White), 1);
           //resampleBackgroundButton.Image = image;
       }

       private void CreateVideoWindows()
       {
           _mainDisplay = new VideoDisplay("Main", new Point(25,25));
           _movementDisplay = new VideoDisplay("Movement", new Point(50 + _mainDisplay.Width + _mainDisplay.Location.X, 25));
           _backgroundDisplay = new VideoDisplay("Background (average)", new Point(50 + _movementDisplay.Width + _movementDisplay.Location.X, 25));
           //_mixtureDisplay = new VideoDisplay("Background (MoG)", new Point(50 + _backgroundDisplay.Width + _backgroundDisplay.Location.X, 25));
           //_mixtureMovementDisplay = new VideoDisplay("Movement (MoG)", new Point(50 + _mixtureDisplay.Width + _mixtureDisplay.Location.X, 25));
           _mainDisplay.Show();
           _movementDisplay.Show();
           _backgroundDisplay.Show();
           //_mixtureDisplay.Show();
           //_mixtureMovementDisplay.Show();
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

       private void AddCamera(ICaptureSource camera)
       {
           _cameras.Add(camera);
           CameraComboBox.Items.Add(camera.Name);
       }

       /// <summary>
       /// Method for initializing the camera selection combobox.
       /// </summary>
       /// <param name="filename">Local file with video</param>
       void InitializeCameraSelection(string filename)
      {
          // Add video file as source, if provided
          if (UseLocalVideo(filename))
          {
              AddCamera(new VideoFileCapture(filename));
          }

          //List all video input devices.
          DsDevice[] systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

          //Variable to indicate the device�s index.
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

      }

       /// <summary>
       /// Try to detect unit tests. Play unit tests (if detected).
       /// </summary>
       /// <param name="assemblyName">Assembly name with test scenarios.</param>
       /// <returns><c>true</c> if unit tests detected.</returns>
       /// <remarks>IMPORTANT: unit tests use own settings!!!</remarks>
       private bool DetectTestScenarios(string assemblyName)
       {
           bool result = false;

           try
           {
               while (! string.IsNullOrWhiteSpace(assemblyName))
               {
                   if (! File.Exists(assemblyName)) break;

                   var assembly = Assembly.LoadFile(assemblyName);
                   if (assembly == null) break;

                   _testCaptureContexts = assembly.GetTypes()
                       .Where(t => t.GetInterfaces().Contains(typeof (ICaptureContextProvider))
                                   && (t.GetConstructor(Type.EmptyTypes) != null)) // expected default constructor
                       .Select(t => Activator.CreateInstance(t) as ICaptureContextProvider)
                       .SelectMany(instance => instance.GetCaptures())
                       .ToArray();

                   foreach (var captureContext in _testCaptureContexts)
                   {
                       AddCamera(captureContext.Capture);
                   }

                   result = true;
                   break;
               }
           }
           catch (Exception e)
           {
               Log(e.ToString());
           }

           return result;
       }

       /// <summary>
      /// Method which reacts to the change of the camera selection combobox.
      /// </summary>
      private void CameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
        //Change the capture device.
        SelectedCamera = _cameras[CameraComboBox.SelectedIndex];   

          // TODO: change settings if app in unit tests visualization mode
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

              System.Threading.Thread.Sleep(33);
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
           Image<Bgr, byte> stateImage = _vista.GetCurrentStateImage();
           Image<Bgr, float> backgroundImage = _vista.GetBackgroundImage(showPolygonsCheckbox.Checked);
           Image<Bgr, float> mogImage = _vista.BackgroundUpdateMoG;

           _mainDisplay.Update(stateImage);
           _backgroundDisplay.Update(backgroundImage);
           //_mixtureDisplay.Update(mogImage);

          if (_vista.Movement_Mask != null)
          {
              Image<Bgr, Byte> movementTimesImage = frame.And(_vista.Movement_Mask.Convert<Bgr, byte>());
              _movementDisplay.Update(movementTimesImage);
          }

           //if (_vista.Movement_MaskMoG != null)
           //{
           //    Image<Bgr, Byte> movementTimesImageMoG = frame.And(_vista.Movement_MaskMoG.Convert<Bgr, byte>());
           //    _mixtureMovementDisplay.Update(movementTimesImageMoG);
           //}
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
       private readonly bool _unitTestsMode;

       private void updateSamplePoint_Click(object sender, EventArgs e)
       {
           string text = rgbCoordinateTextbox.Text;
           string first = text.Split(',')[0];
           string second = text.Split(',')[1];
           RGBSamplePoint.X = Convert.ToInt32(first);
           RGBSamplePoint.Y = Convert.ToInt32(second);
       }
   }
}