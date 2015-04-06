using Emgu.CV;

namespace VTC.CaptureSource
{
    public class IpCamera : CaptureSource
    {
        private string ConnectionString;

        public IpCamera(string name, string connectionString) : base(name)
        {
            ConnectionString = connectionString;
        }

        protected override Capture GetCapture()
        {
            return new Capture(ConnectionString);
        }
    }
}
