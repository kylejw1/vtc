using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTC
{
    public class VideoFileCapture : CaptureSource
    {
        private string Path;

        public VideoFileCapture(string name, string path) : base(name)
        {
            Path = path;
        }

        protected override Capture GetCapture()
        {
            return new Capture(Path);
        }
    }
}
