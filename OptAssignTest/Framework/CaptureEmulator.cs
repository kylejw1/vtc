using System;
using System.Drawing;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using VTC.Kernel.Video;
using VTC.Common;

namespace OptAssignTest.Framework
{
    class CaptureEmulator : ICaptureSource
    {
        private readonly string _name;
        private readonly Script _script;
        private ISettings _settings;
        private Image<Bgr, byte> _background;
        private uint _frame;

        public string Name
        {
            get { return _name; }
        }

        public int Width
        {
            get
            {
                if (_settings == null) throw new ApplicationException("Not initialized properly.");
                return (int)_settings.FrameWidth;
            }
        }

        public int Height
        {
            get
            {
                if (_settings == null) throw new ApplicationException("Not initialized properly.");
                return (int) _settings.FrameHeight;
            }
        }

        public Image<Bgr, byte> QueryFrame()
        {
            if (_settings == null) throw new ApplicationException("Not initialized properly.");

            Image<Bgr, byte> image;
            if (_frame == 0)
            {
                image = _background.Clone(); // check - should it be cloned?
            }
            else
            {
                image = _background.Clone();
                _script.Draw(_frame, image);
            }

            // start script again
            if (_script.IsDone(_frame))
            {
                _frame = 0;
            }
            else
            {
                _frame++;
            }

            Thread.Sleep(0);

            return image;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Capture name.</param>
        /// <param name="script">Underlying script.</param>
        public CaptureEmulator(string name, Script script)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (script == null) throw new ArgumentNullException("script");

            _name = name;
            _script = script;
        }

        public void Init(ISettings settings)
        {
            _settings = settings;
            _background = new Image<Bgr, byte>((int)settings.FrameWidth, (int)settings.FrameHeight, new Bgr(Color.Black));
            _frame = 0;
        }

        public void Destroy()
        {
        }
    }
}
