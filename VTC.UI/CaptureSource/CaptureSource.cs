using System;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using VTC.Common;
using VTC.Kernel.Video;

namespace VTC.CaptureSource
{
    public abstract class CaptureSource : Kernel.Video.ICaptureSource
    {
        private readonly string _name;
        private Capture _cameraCapture;

        public int Width
        {
            get
            {
                if (_cameraCapture == null) throw new ApplicationException("Camera is not initialized.");
                return _cameraCapture.Width;
            }
        }

        public int Height
        {
            get
            {
                if (_cameraCapture == null) throw new ApplicationException("Camera is not initialized.");
                return _cameraCapture.Height;
            }
        }

        public string Name
        {
            get { return _name; }
        }


        protected abstract Capture GetCapture();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Capture source name.</param>
        protected CaptureSource(string name)
        {
            _name = name;
        }

        public delegate void OnCaptureComplete();
        public OnCaptureComplete captureCompleteEvent;

        /// <summary>
        /// Get next frame from camera.
        /// </summary>
        /// <returns></returns>
        public Image<Bgr, Byte> QueryFrame()
        {
            var frame = _cameraCapture.QueryFrame();
            if (frame == null)
            {
                if (_cameraCapture.CaptureSource == Capture.CaptureModuleType.Camera)
                {
                    _cameraCapture.Stop();
                    _cameraCapture.Start();
                    Debug.WriteLine("Restarting camera: " + DateTime.Now);
                }
                else
                {
                    //Video is finished
                    captureCompleteEvent?.Invoke();
                }

                // Return a dummy image
                return new Image<Bgr, byte>(640, 480, new Bgr(System.Drawing.Color.White));
            }
            return frame.ToImage<Bgr,byte>();
        }

        /// <summary>
        /// Initialize camera.
        /// </summary>
        /// <param name="settings"></param>
        public void Init(ISettings settings)
        {
            _cameraCapture = GetCapture();
            
            _cameraCapture.SetCaptureProperty(CapProp.FrameHeight, settings.FrameHeight);
            _cameraCapture.SetCaptureProperty(CapProp.FrameWidth, settings.FrameWidth);
        }

        /// <summary>
        /// Destroy underlying camera.
        /// </summary>
        public void Destroy()
        {
            if (null != _cameraCapture)
            {
                _cameraCapture.Dispose();
                _cameraCapture = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
