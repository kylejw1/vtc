using System.IO;
using Emgu.CV;

namespace VTC
{
    public class VideoFileCapture : CaptureSource
    {
        private readonly string _path;

        public VideoFileCapture(string path)
            : base("File: " + Path.GetFileName(path))
        {
            _path = path;
        }

        protected override Capture GetCapture()
        {
            return new Capture(_path);
        }
    }
}
