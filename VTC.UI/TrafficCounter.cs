using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using VTC.Kernel.RegionConfig;
using VTC.Kernel.Video;
using VTC.Kernel.Vistas;
using VTC.Reporting;
using VTC.Reporting.ReportItems;
using VTC.Common;
using VTC.BatchProcessing;
using VTC.RegionConfiguration;

namespace VTC
{
   public partial class TrafficCounter : Form
   {
        #region VideoDisplays
        private VideoDisplay _mainDisplay;
        private VideoDisplay _movementDisplay;
        private VideoDisplay _backgroundDisplay;
        private VideoDisplay _velocityFieldDisplay;
        private VideoDisplay _mixtureDisplay;
        private VideoDisplay _mixtureMovementDisplay;
        private VideoDisplay _3DPointsDisplay;
        private VideoMux _videoMux;
        #endregion

        #region Mode Flags
        private bool _batchMode;
        private readonly bool _unitTestsMode;
        private readonly bool _isLicensed;
        #endregion

        private Vista _vista;

        private static readonly Logger Logger = LogManager.GetLogger("main.form");

        private readonly AppSettings _settings;
        private const string IpCamerasFilename = "ipCameras.txt";

        private readonly DateTime _applicationStartTime;
        private DateTime _lastDatasetExportTime;

        private readonly List<ICaptureSource> _cameras = new List<ICaptureSource>(); //List of all video input devices. Index, file location, name
        private ICaptureSource _selectedCamera;

        private static readonly string RegionConfigSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                            "\\VTC\\regionConfig.xml";
        private IRegionConfigDataAccessLayer _regionConfigDataAccessLayer = new FileRegionConfigDAL(RegionConfigSavePath);
        private List<RegionConfig> _regionConfigs;

        private List<BatchVideoJob> _videoJobs; 

        private readonly string _appArgument; //For debugging only, delete this later
        private TimeSpan _trialLicenseTimeLimit = TimeSpan.FromMinutes(30);
       private Int64 numProcessedFrames = 0;

        // unit tests has own settings, so need to store "pairs" (capture, settings)
        private CaptureContext[] _testCaptureContexts;

        private string GetVideoSource()
       {
           return SelectedCamera.Name;
       }

        /// <summary>
       /// Active camera.
       /// </summary>
        private ICaptureSource SelectedCamera
      {
          get { return _selectedCamera; }
          set
          {
              if (value == _selectedCamera) return;

              _selectedCamera?.Destroy();

              _selectedCamera = value;
              _selectedCamera.Init(_settings);

              _vista = new IntersectionVista(_settings, _selectedCamera.Width, _selectedCamera.Height, _regionConfigs.FirstOrDefault())
              {
                  GetCameraSource = GetVideoSource
              };

              SetImageBoxesToVideoSize();

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

        /// <summary>
       /// Constructor.
       /// </summary>
       /// <param name="settings">Application settings.</param>
       /// <param name="isLicensed">If false, software will shut down after a few minutes</param>
       /// <param name="appArgument">Can mean different things (Local file with video, Unit tests, etc).</param>
        public TrafficCounter(AppSettings settings, bool isLicensed, string appArgument = null)
       {           
           _appArgument = appArgument; 

          InitializeComponent();

            _settings = settings;
           _isLicensed = isLicensed;

           // check if app should run in unit test visualization mode
           _unitTestsMode = false;
           if ("-tests".Equals(appArgument, StringComparison.OrdinalIgnoreCase))
           {
               _unitTestsMode = DetectTestScenarios(settings.UnitTestsDll);
           }

            _regionConfigs = _regionConfigDataAccessLayer.LoadRegionConfigList().ToList();

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
          LoadConfiguration();

           //Create video windows
           CreateVideoWindows();

           _applicationStartTime = DateTime.Now;
           _lastDatasetExportTime = DateTime.Now;

            _videoJobs = new List<BatchVideoJob>();

           DisableExperimental();

           Run();
      }

        private void Run()
      {
         try
         {
            SelectedCamera = _cameras.First();
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
         //    throw;
         }
         
         Application.Idle += ProcessFrame;

         var intersectionImagePath = "ftp://"+ _settings.ServerUrl +"/intersection_" + _settings.IntersectionID + ".png";
         Reporter.INSTANCE.AddReportItem(
             new FtpSendFileReportItem(
                 _settings.FrameUploadIntervalMinutes, 
                 new Uri(intersectionImagePath),
                  new NetworkCredential(_settings.FtpUserName, _settings.FtpPassword),
                  GetCameraFrameBytes
                 ));

         Reporter.INSTANCE.Start(); //TODO: Figure out a better way of synchronizing this state with UI (checkbox).

      }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (null == SelectedCamera)
                return;

            using (var frame = SelectedCamera.QueryFrame())
            {
                try
                {
                    //Workaround - dispose and restart capture if camera starts returning null. 
                    //TODO: Investigate - why is this necessary?
                    if (frame == null)
                    {
                        if (_batchMode)
                        {
                            if ( _videoJobs.Count > 0)
                                DequeueVideo();
                        }
                        else
                        {
                            SelectedCamera = new VideoFileCapture(_appArgument);
                            Debug.WriteLine("Restarting camera: " + DateTime.Now);
                        }
                    }
                    else
                    {
                        var frameForProcessing = frame.Clone();
                        // Necessary so that frame.Data becomes accessible
                        var frameForRendering = frame.Clone();

                        // Send the new image frame to the vista for processing
                        _vista.DisableOpticalFlow = disableOpticalFlowCheckbox.Checked;
                        _vista.Update(frameForProcessing, mhtLogCheckbox.Checked);

                        // Update image boxes
                        UpdateImageBoxes(frameForRendering, _vista.Movement_Mask);

                        // Update statistics
                        tbVistaStats.Text = _vista.GetStatString();

                        //Thread.Sleep((int)_settings.Timestep * 1000); 
                        var activeTime = DateTime.Now - _applicationStartTime;
                        timeActiveTextBox.Text = activeTime.ToString(@"dd\.hh\:mm\:ss");

                        //Export training images
                        if (numProcessedFrames % 10 == 0)
                            TryExportDataset(frameForProcessing);

                        if (delayProcessingCheckbox.Checked)
                            Thread.Sleep(500);

                        if (activeTime > _trialLicenseTimeLimit && !_isLicensed)
                            NotifyLicenseAndExit();

                        numProcessedFrames++;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    throw;
#else
                    Logger.Log(LogLevel.Error, "In Vista:Update(), " + ex.Message);
#endif
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
           var result = false;

           try
           {
               while (! string.IsNullOrWhiteSpace(assemblyName))
               {
                   // ensure absolute path
                   if (! Path.IsPathRooted(assemblyName))
                   {
                       var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                       if(currentDir != null)
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
     
        private void LoadConfiguration()
      {
          intersectionIDTextBox.Text = _settings.IntersectionID;
          pushStateTimer.Interval = _settings.StateUploadIntervalMs;
          serverUrlTextBox.Text = _settings.ServerUrl;
      }

        /// <summary>
       /// Use this function to terminate the application if a trial license timeout occurs
       /// </summary>
        private void NotifyLicenseAndExit()
       {
           MessageBox.Show(
               "This is only a trial version! Please visit www.traffic-camera.com to use software longer than " + _trialLicenseTimeLimit.Minutes.ToString() + " minutes.");
           Application.Exit();
       }

        private void heartbeatTimer_Tick(object sender, EventArgs e)
        {

            var heartbeatPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VTC\\heartbeat";
            var heartbeatFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VTC";

            if (!Directory.Exists(heartbeatFolderPath))
                Directory.CreateDirectory(heartbeatFolderPath);

            if (!File.Exists(heartbeatPath))
                File.Create(heartbeatPath).Close();

            File.SetLastWriteTime(heartbeatPath, DateTime.Now);
        }

        #region Camera

        private void ResampleBackground(Image<Bgr, byte> frame)
        {
            _vista.InitializeBackground(frame);
        }

        private void AddCamera(ICaptureSource camera)
        {
            _cameras.Add(camera);
            CameraComboBox.Items.Add(camera.Name);
        }

        private void InitializeCameraSelection(string filename)
        {
            // Add video file as source, if provided
            if (UseLocalVideo(filename))
            {
                LoadCameraFromFilename(filename);
            }

            //List all video input devices.
            var systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

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
                foreach (var split in ipCameraStrings.Select(str => str.Split(',')).Where(split => split.Length == 2))
                    AddCamera(new IpCamera(split[0], split[1]));
            }
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

        #endregion

        #region Logging, Measurement and Export

        private static void Log(LogLevel logLevel, string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Logger.Log(logLevel, format, args);
        }

        private static void Log(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        /// <summary>
        /// Get the current frame in the form of a PNG byte array
        /// </summary>
        /// <returns>Byte array containing the current frame in PNG format</returns>
        private byte[] GetCameraFrameBytes()
        {
            using (var bmp = _vista.ColorBackground.ToBitmap())
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                stream.Close();

                return stream.ToArray();
            }
        }

        private void PushStateProcess(object sender, EventArgs e)
        {
            if (pushStateCheckbox.Checked)
            {
                var pushThread = new Thread(PushState);
                pushThread.Start();
            }
        }

        private void PushState()
        {
            var success = false;
            try
            {
                var stateEstimates = _vista.CurrentVehicles.Select(v => v.StateHistory.Last()).ToArray();
                string postString;
                var postUrl = HttpPostReportItem.PostStateString(stateEstimates, _settings.IntersectionID, _settings.ServerUrl, out postString);
                HttpPostReportItem.SendStatePost(postUrl, postString);
                success = true;
            }
            catch (Exception ex)
            {
#if (DEBUG)
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

        private void TryExportDataset(Image<Bgr, byte> frame)
        {
            _lastDatasetExportTime = DateTime.Now;
            if (exportDatasetsCheckbox.Checked)
            {
                var eT = new ExportTrainingSet.ExportTrainingSet(_settings,
                    frame.Convert<Bgr, float>(), _vista.CurrentVehicles, _vista.Movement_Mask);
                //eT.autoExportScaledPositives(); //TODO: Make this an option, not just commented out
                //eT.autoExportScaledMasks();
                //eT.autoExportMasks();
                eT.autoExportDualImages();
            }
        }
        #endregion

        #region Batch Processing

        private VideoFileCapture LoadCameraFromFilename(string filename)
        {
            var vfc = new VideoFileCapture(filename);
            vfc.captureCompleteEvent += NotifyProcessingComplete;
            AddCamera(vfc);
            return vfc;
        }

        private void NotifyProcessingComplete()
        {
            infoBox.AppendText("Video complete" + Environment.NewLine);
            DequeueVideo();
        }

        private void LoadVideosFromPath(List<BatchVideoJob> videoJobs)
        {
            _batchMode = true;
            _cameras.Clear();
            CameraComboBox.Items.Clear();
            _videoJobs = videoJobs;
            DequeueVideo();
        }

        private void DequeueVideo()
        {
            if (_videoJobs.Count > 0)
            {
                infoBox.AppendText("Loading video from batch" + Environment.NewLine);
                SelectedCamera = LoadCameraFromFilename(_videoJobs.First().VideoPath);
                _videoJobs.Remove(_videoJobs.First());
            }
            else
            {
                infoBox.AppendText("Batch complete." + Environment.NewLine);
                infoBox.AppendText("Movement counts saved to desktop." + Environment.NewLine);

                Application.Idle -= ProcessFrame;
            }
        }
        #endregion

        #region Rendering

        private void SetImageBoxesToVideoSize()
        {
            var videoSize = new Size(SelectedCamera.Width, SelectedCamera.Height);

            _mainDisplay.Size = videoSize;
            _mainDisplay.ImageBox.SetZoomScale(1.0, new Point(0, 0));
            _backgroundDisplay.Size = videoSize;
            _movementDisplay.Size = videoSize;
            _videoMux.Size = MaximumSize;
#if DEBUG

            _mixtureDisplay.Size = videoSize;
            _mixtureDisplay.ImageBox.SetZoomScale(1.0, new Point(0, 0));

            _mixtureMovementDisplay.Size = videoSize;
            _mixtureMovementDisplay.ImageBox.SetZoomScale(1.0, new Point(0, 0));

            _3DPointsDisplay.Size = videoSize;
            _3DPointsDisplay.ImageBox.SetZoomScale(1.0, new Point(0, 0));

            _velocityFieldDisplay.Size = videoSize;
            _velocityFieldDisplay.ImageBox.SetZoomScale(1.0, new Point(0, 0));
#endif
        }

        //Hide UI for functionality under development when in release mode
        private void DisableExperimental()
        {
#if !DEBUG
           intersectionIDTextBox.Visible = false;
           intersectionIDLabel.Visible = false;
           serverURLLabel.Visible = false;
           serverUrlTextBox.Visible = false;
           pushStateCheckbox.Visible = false;
           exportDatasetsCheckbox.Visible = false;
           mhtLogCheckbox.Visible = false;
           disableOpticalFlowCheckbox.Visible = false;
           MoGcheckBox.Visible = false;
           delayProcessingCheckbox.Visible = false;
#endif
        }

        private void CreateVideoWindows()
        {
            _mainDisplay = new VideoDisplay("Main", new Point(25, 25));
            _movementDisplay = new VideoDisplay("Movement", new Point(50 + _mainDisplay.Width + _mainDisplay.Location.X, 25));
            _backgroundDisplay = new VideoDisplay("Background (average)", new Point(50 + _movementDisplay.Width + _movementDisplay.Location.X, 25));

#if DEBUG
            _velocityFieldDisplay = new VideoDisplay("Velocity Field", new Point(_movementDisplay.Location.X, _movementDisplay.Location.Y + _movementDisplay.Size.Height));
            _mixtureDisplay = new VideoDisplay("Background (MoG)", new Point(50 + _backgroundDisplay.Width + _backgroundDisplay.Location.X, 25));
            _mixtureMovementDisplay = new VideoDisplay("Movement (MoG)", new Point(50 + _mixtureDisplay.Width + _mixtureDisplay.Location.X, 25));
            _3DPointsDisplay = new VideoDisplay("3D Points", new Point(50 + _mixtureMovementDisplay.Width + _mixtureMovementDisplay.Location.X, 25));
#endif

            _videoMux = new VideoMux();
            _videoMux.AddDisplay(_mainDisplay.ImageBox, _mainDisplay.LayerName);
            _videoMux.AddDisplay(_movementDisplay.ImageBox, _movementDisplay.LayerName);
            _videoMux.AddDisplay(_backgroundDisplay.ImageBox, _backgroundDisplay.LayerName);

#if DEBUG
            _videoMux.AddDisplay(_velocityFieldDisplay.ImageBox, _velocityFieldDisplay.LayerName);
            _videoMux.AddDisplay(_mixtureDisplay.ImageBox, _mixtureDisplay.LayerName);
            _videoMux.AddDisplay(_mixtureMovementDisplay.ImageBox, _mixtureMovementDisplay.LayerName);
            _videoMux.AddDisplay(_3DPointsDisplay.ImageBox, _3DPointsDisplay.LayerName);
#else
            _videoMux._displayLookup.ElementAt(1).Key.Checked = false;
           _videoMux._displayLookup.ElementAt(2).Key.Checked = false;
#endif
            _videoMux.Show();
        }

        private void UpdateImageBoxes(Image<Bgr, byte> frame, Image<Gray, byte> movementMask)
        {
            var stateImage = _vista.GetCurrentStateImage(frame);
            var backgroundImage = _vista.GetBackgroundImage(showPolygonsCheckbox.Checked);

            _mainDisplay.Update(stateImage);
            _backgroundDisplay.Update(backgroundImage);

            var movementTimesImage = frame.And(movementMask.Convert<Bgr, byte>());
            _movementDisplay.Update(movementTimesImage);

#if DEBUG
            var mogImage = _vista.MoGBackgroundSingleton.BackgroundImage();
            _mixtureDisplay.Update(mogImage);

            if (MoGcheckBox.Checked)
            {
                var movementTimesImageMoG = frame.And(movementMask.Convert<Bgr, byte>());
                _mixtureMovementDisplay.Update(movementTimesImageMoG);
            }

            if (_vista != null)
            {
                var pointsImage = Render3DPoints();
                _3DPointsDisplay.Update(pointsImage);
                var image = new Image<Bgr, byte>(800, 600);
                _vista.DrawVelocityField(image, new Bgr(Color.White), 1);
                _velocityFieldDisplay.Update(image);
            }
#endif


        }

        private Image<Bgr, byte> Render3DPoints()
        {
            var pointsImage = new Image<Bgr, byte>(new Size(_vista._frame.Width, _vista._frame.Height));
            var measurementsArrayArray = _vista.MeasurementArrayQueue.ToArray();
            var totalLength = measurementsArrayArray.Length;

            for (int i = 0; i < totalLength; i++)
                for (int j = 0; j < measurementsArrayArray[i].Length; j++)
                {
                    var p = measurementsArrayArray[i][j];
                    var intensity = (double)(totalLength - i) / totalLength;
                    intensity = 1 - intensity;
                    var color = new Bgr(p.Blue * intensity, p.Green * intensity, p.Red * intensity);
                    pointsImage.Draw(new CircleF(new PointF((float)p.X, (float)p.Y), (float)1.0), color);
                }

            return pointsImage;
        }
        #endregion

        #region Click Handlers

        private void TrafficCounter_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Idle -= ProcessFrame;
        }

        private void pushStateCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (pushStateCheckbox.Checked)
            {
                Reporter.INSTANCE.Start();
            }
            else
            {
                Reporter.INSTANCE.Stop();
            }
        }

        private void MoGcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vista.EnableMoG = MoGcheckBox.Checked;
        }

        private void resampleBackgroundButton_Click(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> frame = SelectedCamera.QueryFrame())
            {
                if (frame != null)
                    ResampleBackground(frame);
            }
        }

        /// <summary>
        /// Method which reacts to the change of the camera selection combobox.
        /// </summary>
        private void CameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Change the capture device.
            SelectedCamera = _cameras[CameraComboBox.SelectedIndex];
            Application.Idle -= ProcessFrame;
            Application.Idle += ProcessFrame;

            // TODO: change settings if app in unit tests visualization mode
        }

        private void btnToggleVideoMux_Click(object sender, EventArgs e)
        {
            if (null == _videoMux || _videoMux.IsDisposed)
            {
                CreateVideoWindows();
                _videoMux?.Show();
                return;
            }

            if (_videoMux.Visible)
            {
                _videoMux.Hide();
            }
            else
            {
                _videoMux.Show();
            }
        }

        private void btnConfigureRegions_Click(object sender, EventArgs e)
        {
            var r = new RegionEditor(_vista.ColorBackground, _vista.RegionConfiguration);
            if (r.ShowDialog() == DialogResult.OK)
            {
                _vista.RegionConfiguration = r.RegionConfig;

                _regionConfigs.Remove(_vista.RegionConfiguration);
                _regionConfigs.Add(r.RegionConfig);

                _regionConfigDataAccessLayer.SaveRegionConfigList(_regionConfigs);

                foreach (var reg in r.RegionConfig.Regions.Where(reg => reg.Value.PolygonClosed))
                    reg.Value.UpdateCentroid();
            }
            else
            {
                MessageBox.Show("Region configuration failed");
            }

        }

        private void SelectVideosButton_Click(object sender, EventArgs e)
        {
            var dr = selectVideoFilesDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _videoJobs = new List<BatchVideoJob>();
                var videoPathsList = selectVideoFilesDialog.FileNames.ToList();
                foreach (var job in videoPathsList.Select(p => new BatchVideoJob {VideoPath = p}))
                    _videoJobs.Add(job);
                
                LoadVideosFromPath(_videoJobs);
            }
                
            Application.Idle -= ProcessFrame;
            Application.Idle += ProcessFrame;
        }
        #endregion

    }
}