using Emgu.CV;
using System;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using VTC.Kernel.Settings;

namespace VTC
{
    public abstract class CaptureSource
    {
        private readonly string Name;
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


        protected abstract Capture GetCapture();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Capture source name.</param>
        protected CaptureSource(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Get next frame from camera.
        /// </summary>
        /// <returns></returns>
        public Image<Bgr, Byte> QueryFrame()
        {
            return _cameraCapture.QueryFrame();
        }

        /// <summary>
        /// Initialize camera.
        /// </summary>
        /// <param name="settings"></param>
        public void Init(ISettings settings)
        {
            _cameraCapture = GetCapture();

            _cameraCapture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, settings.FrameHeight);
            _cameraCapture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, settings.FrameWidth);
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
