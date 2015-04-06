using Emgu.CV;

namespace VTC.CaptureSource
{
    public class SystemCamera : CaptureSource
    {
        private int SystemDeviceIndex;

        public SystemCamera(string name, int systemDeviceIndex) : base(name)
        {
            SystemDeviceIndex = systemDeviceIndex;
        }

        protected override Capture GetCapture()
        {
            return new Capture(SystemDeviceIndex);
        }
    }
}
