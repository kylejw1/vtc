using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTC
{
    public class IpCamera : CaptureSource
    {
        private string ConnectionString;

        public IpCamera(string name, string connectionString) : base(name)
        {
            ConnectionString = connectionString;
        }

        public override Capture GetCapture()
        {
            return new Capture(ConnectionString);
        }
    }
}
