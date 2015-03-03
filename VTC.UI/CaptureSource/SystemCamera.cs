using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTC
{
    public class SystemCamera : CaptureSource
    {
        private int SystemDeviceIndex;

        public SystemCamera(string name, int systemDeviceIndex) : base(name)
        {
            SystemDeviceIndex = systemDeviceIndex;
        }

        public override Capture GetCapture()
        {
            return new Capture(SystemDeviceIndex);
        }
    }
}
