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
using NLog;
using VTC.CaptureSource;
using VTC.Kernel;
using VTC.Kernel.RegionConfig;
using VTC.Kernel.Video;
using VTC.Kernel.Vistas;
using VTC.Reporting;
using VTC.Reporting.ReportItems;
using VTC.Settings;
using SkyXoft.BusinessSolutions.LicenseManager.Protector;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
       private static readonly Logger _logger = LogManager.GetLogger("main.form");

       private readonly AppSettings _settings;
       private const string IpCamerasFilename = "ipCameras.txt";

       private VideoDisplay _mainDisplay;
       private VideoDisplay _movementDisplay;
       private VideoDisplay _backgroundDisplay;
       private VideoDisplay _velocityFieldDisplay;
       private VideoDisplay _velocityProjectDisplay;
       private VideoDisplay _mixtureDisplay;
       private VideoDisplay _mixtureMovementDisplay;
       private VideoDisplay _3DPointsDisplay;
       private VideoDisplay _opticalFlowDisplay;
	   private VideoMux _videoMux;

       private readonly DateTime _applicationStartTime;
       private DateTime _lastDatasetExportTime;
       ExtendedLicense license;
       bool isActivated;


      private readonly List<ICaptureSource> _cameras = new List<ICaptureSource>(); //List of all video input devices. Index, file location, name
      private ICaptureSource _selectedCamera;

       private string _appArgument; //For debugging only, delete this later

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

       /// <summary>
       /// Constructor.
       /// </summary>
       /// <param name="settings">Application settings.</param>
       /// <param name="appArgument">Can mean different things (Local file with video, Unit tests, etc).</param>
       public TrafficCounter(AppSettings settings, string appArgument = null)
       {
           ValidateLicense();

           if (isActivated)
               MessageBox.Show("License validated");
           else
               MessageBox.Show("License invalid");

           _appArgument = appArgument;
          InitializeComponent();

           _settings = settings;

           // check if app should run in unit test visualization mode
           _unitTestsMode = false;
           if ("-tests".Equals(appArgument, StringComparison.OrdinalIgnoreCase))
           {
               _unitTestsMode = DetectTestScenarios(settings.UnitTestsDll);
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

           System.Windows.Forms.Timer t = new System.Windows.Forms.Timer {Interval = 1000};
           t.Tick += OnTick;
           t.Start();

           //Create video windows
           CreateVideoWindows();

           _applicationStartTime = DateTime.Now;
           _lastDatasetExportTime = DateTime.Now;
           
           Run();
      }


       //TODO: Move this to the Render Frames (??) function where everything else is drawn. 
       private void OnTick(object sender, EventArgs eventArgs)
       {
           if (_vista != null)
           {
               var image = new Image<Bgr, Byte>(800, 600);
               _vista.DrawVelocityField(image, new Bgr(Color.White), 1);
               _velocityFieldDisplay.Update(image);
               Image<Gray, byte> pimage;
               pimage = _vista.VelocityProjection();
               _velocityProjectDisplay.Update(pimage.Convert<Bgr, byte>());
           }
       }

       private void CreateVideoWindows()
       {
           _mainDisplay = new VideoDisplay("Main", new Point(25,25));
           _movementDisplay = new VideoDisplay("Movement", new Point(50 + _mainDisplay.Width + _mainDisplay.Location.X, 25));
           _backgroundDisplay = new VideoDisplay("Background (average)", new Point(50 + _movementDisplay.Width + _movementDisplay.Location.X, 25));
           _velocityFieldDisplay = new VideoDisplay("Velocity Field", new Point(_movementDisplay.Location.X, _movementDisplay.Location.Y + _movementDisplay.Size.Height));
           _velocityProjectDisplay = new VideoDisplay("Velocity Projection", new Point(_backgroundDisplay.Location.X, _backgroundDisplay.Location.Y + _backgroundDisplay.Size.Height));
           _mixtureDisplay = new VideoDisplay("Background (MoG)", new Point(50 + _backgroundDisplay.Width + _backgroundDisplay.Location.X, 25));
           _mixtureMovementDisplay = new VideoDisplay("Movement (MoG)", new Point(50 + _mixtureDisplay.Width + _mixtureDisplay.Location.X, 25));
           _3DPointsDisplay = new VideoDisplay("3D Points", new Point(50 + _mixtureMovementDisplay.Width + _mixtureMovementDisplay.Location.X, 25));
           _opticalFlowDisplay = new VideoDisplay("Optical Flow", new Point(50 + _mixtureMovementDisplay.Width + _mixtureMovementDisplay.Location.X, 25));
		   
		   
		    _videoMux = new VideoMux();
            _videoMux.AddDisplay(_mainDisplay.ImageBox, _mainDisplay.LayerName);
			_videoMux.AddDisplay(_movementDisplay.ImageBox, _movementDisplay.LayerName);
			_videoMux.AddDisplay(_backgroundDisplay.ImageBox, _backgroundDisplay.LayerName);
			_videoMux.AddDisplay(_velocityFieldDisplay.ImageBox, _velocityFieldDisplay.LayerName);
			_videoMux.AddDisplay(_velocityProjectDisplay.ImageBox, _velocityProjectDisplay.LayerName);
			_videoMux.AddDisplay(_mixtureDisplay.ImageBox, _mixtureDisplay.LayerName);
			_videoMux.AddDisplay(_mixtureMovementDisplay.ImageBox, _mixtureMovementDisplay.LayerName);
            _videoMux.AddDisplay(_3DPointsDisplay.ImageBox, _3DPointsDisplay.LayerName);
            _videoMux.AddDisplay(_opticalFlowDisplay.ImageBox, _opticalFlowDisplay.LayerName);
			_videoMux.Show();
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
             throw;
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

         ServerReporter.INSTANCE.Start(); //TODO: Figure out a better way of synchronizing this state with UI (checkbox).

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
                   // ensure absolute path
                   if (! Path.IsPathRooted(assemblyName))
                   {
                       var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                       assemblyName = Path.Combine(currentDir, assemblyName);
                   }

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

                   Log(LogLevel.Trace, "Test mode detected.");
                   result = true;
                   break;
               }
           }
           catch (Exception e)
           {
               Log(LogLevel.Error, e.ToString());
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
              Log(LogLevel.Error, message);
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
                      
                      SelectedCamera = new VideoFileCapture(_appArgument);

                      Debug.WriteLine("Restarting camera: " + DateTime.Now);
                      return;
                  }

                  Image<Bgr, Byte> frameForProcessing = frame.Clone(); // Necessary so that frame.Data becomes accessible
                  Image<Bgr, Byte> frameForRendering = frame.Clone();

                  // Send the new image frame to the vista for processing
                  _vista.DisableOpticalFlow = disableOpticalFlowCheckbox.Checked;
                  _vista.Update(frameForProcessing);

                  // Update image boxes
                  UpdateImageBoxes(frameForRendering);

                  // Update statistics
                  trackCountBox.Text = _vista.CurrentVehicles.Count.ToString();
                  tbVistaStats.Text = _vista.GetStatString();

                  Thread.Sleep((int)_settings.Timestep * 1000); //TODO: don't use a hardcoded wait here
                  TimeSpan activeTime = (DateTime.Now - _applicationStartTime);
                  timeActiveTextBox.Text = activeTime.ToString(@"dd\.hh\:mm\:ss");

                  //Export training images
                  if(DateTime.Now - _lastDatasetExportTime > TimeSpan.FromSeconds(10))
                  tryExportDataset(frameForProcessing);

                  if (delayProcessingCheckbox.Checked)
                      Thread.Sleep(500);

              }   
      }

       private void UpdateImageBoxes(Image<Bgr, Byte> frame)
       {
           Image<Bgr, byte> stateImage = _vista.GetCurrentStateImage(frame);
           Image<Bgr, float> backgroundImage = _vista.GetBackgroundImage(showPolygonsCheckbox.Checked);
           Image<Bgr, float> mogImage = _vista.MoGBackgroundSingleton.BackgroundUpdateMoG;
           var opticalFlowImage = RenderOpticalFlowImage();

           //Render 3D points
           Image<Bgr, byte> pointsImage = Render3DPoints();

           _mainDisplay.Update(stateImage);
           _backgroundDisplay.Update(backgroundImage);
           _mixtureDisplay.Update(mogImage);
           _3DPointsDisplay.Update(pointsImage);
           _opticalFlowDisplay.Update(opticalFlowImage);

          if (_vista.Movement_Mask != null)
          {
              Image<Bgr, Byte> movementTimesImage = frame.And(_vista.Movement_Mask.Convert<Bgr, byte>());
              _movementDisplay.Update(movementTimesImage);
          }

          if (_vista.Movement_MaskMoG != null)
          {
              Image<Bgr, Byte> movementTimesImageMoG = frame.And(_vista.Movement_MaskMoG.Convert<Bgr, byte>());
              _mixtureMovementDisplay.Update(movementTimesImageMoG);
          }
      }

       private Image<Bgr, float> RenderOpticalFlowImage()
       {
           Image<Bgr, float> opticalFlowImage = new Image<Bgr, float>(new Size(_vista._frame.Width, _vista._frame.Height));
           if (!disableOpticalFlowCheckbox.Checked)
           {

               for (int i = 0; i < _vista._frame.Width; i++)
                   for (int j = 0; j < _vista._frame.Height; j++)
                   {
                       opticalFlowImage.Data[j, i, 0] = (float)(Math.Abs(_vista.OpticalFlow[i][j][0]));
                       opticalFlowImage.Data[j, i, 1] = (float)(Math.Abs(_vista.OpticalFlow[i][j][1]));
                   }
               opticalFlowImage = opticalFlowImage.Mul(15);
               //_vista.DrawVelocityField(opticalFlowImage, new Bgr(Color.White), 1, _vista.OpticalFlow); 
           }
           return opticalFlowImage;
       }

       private Image<Bgr, byte> Render3DPoints()
       {
           Image<Bgr,byte> pointsImage = new Image<Bgr, byte>(new Size(_vista._frame.Width, _vista._frame.Height));
           var measurementsArrayArray = _vista.MeasurementArrayQueue.ToArray();
           var totalLength = measurementsArrayArray.Length;

           for(int i=0;i< totalLength;i++)
               for (int j = 0; j < measurementsArrayArray[i].Length; j++)
               {
                   var p = measurementsArrayArray[i][j];
                   double intensity = (double) (totalLength - i)/totalLength;
                   intensity = 1 - intensity;
                   Bgr color = new Bgr(p.Blue * intensity, p.Green * intensity, p.Red * intensity);
                   pointsImage.Draw(new CircleF(new PointF((float)p.X,(float)p.Y), (float)1.0),color);
               }
               
           return pointsImage;
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
              StateEstimate[] stateEstimates = _vista.CurrentVehicles.Select(v => v.StateHistory.Last()).ToArray();
              string postString;
              string postUrl = HttpPostReportItem.PostStateString(stateEstimates, _settings.IntersectionID, _settings.ServerUrl, out postString);
              HttpPostReportItem.SendStatePost(postUrl, postString);
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
	  
	  private void btnToggleVideoMux_Click(object sender, EventArgs e)
        {
            if (null == _videoMux || _videoMux.IsDisposed)
            {
                CreateVideoWindows();
                _videoMux.Show();
                return;
            }
            
            if (_videoMux.Visible)
            {
                _videoMux.Hide();
            } else
            {
                _videoMux.Show();
            }
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

       private readonly bool _unitTestsMode;

       

       private void MoGcheckBox_CheckedChanged(object sender, EventArgs e)
       {
           _vista.EnableMoG = MoGcheckBox.Checked;
       }

       private void tryExportDataset(Image<Bgr,byte> frame)
       {
           _lastDatasetExportTime = DateTime.Now;
           if (exportDatasetsCheckbox.Checked)
           {
                   ExportTrainingSet.ExportTrainingSet eT = new ExportTrainingSet.ExportTrainingSet(_settings,
                       frame.Convert<Bgr, float>(), _vista.CurrentVehicles, _vista.Movement_Mask);
                   //eT.autoExportScaledPositives(); //TODO: Make this an option, not just commented out
                   //eT.autoExportScaledMasks();
                   //eT.autoExportMasks();
                   eT.autoExportDualImages();
           }              
       }

       private void watchdogTimer_Tick(object sender, EventArgs e)
       {
            var heartbeatDirPath = ".\\";
            var heartbeatFilePath = heartbeatDirPath + "heartbeat";

            Directory.CreateDirectory(heartbeatDirPath);
            if (!File.Exists(heartbeatFilePath))
                File.Create(heartbeatFilePath).Close();

           File.SetLastWriteTime(heartbeatFilePath, DateTime.Now);
       }

       private void hideTrackersButton_Click(object sender, EventArgs e)
       {
           _vista.hide_trackers = !_vista.hide_trackers;
       }

       private void ValidateLicense()
       {
            try
        {
            license = ExtendedLicenseManager.GetLicense(typeof(TrafficCounter), this, "<RSAKeyValue><Modulus>uHfytqHYNN+1mYDeocM6fjotTwmQgGphb4XaMtrADk3+oa03ZWMXkIFZyL7mzG/hPpd/Q+waSWiklL7QR4k1XujCbcLNngY0gz4qaKFq/LqCSHzX7zHQ3N1Lyg368XK+uLtAxX9fGF9vOgloIPnDb/4Jol6nohouKODSZc+rf43D2q6mYWApWPrBFrhGyeO9mF3khYkFiJTXnCDku8WbJBdwK963RmYkI5p+jyoDi0Uy5a2+TmU9jnzK7zyRybjd4f1o7bfFQlBouSCrwVzU0n8PmtrU5boSh45RbDuy5FRYknxBM9djQvewydLTVHztZWjeQ0Q3JxH03/6DIY0Lsw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"); 
		
            // Check if we're activated, and every 90 days verify it with
            // the activation servers. 
            GenuineResult result = license.IsGenuineEx();

            isActivated = result == GenuineResult.Genuine ||
                          // an internet error means the user is activated but
                          // IPManager failed to contact the LicenseSpot servers
                          result == GenuineResult.InternetError;

            if (result == GenuineResult.InternetError)
            {
                //TODO: give the user the option to retry the genuine checking
                //      immediately. For example a dialog box. In the dialog
                //      call IsGenuineEx() to retry immediately.
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to check if activated: " + ex.Message);
        }
       }

       private void activateLicenseButton_Click(object sender, EventArgs e)
       {
           license.Activate("F884-F8BB-9ED0-4CC7-ACEF-E385-2641");
       }


   }
}