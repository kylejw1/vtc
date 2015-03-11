using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTC
{
    public abstract class CaptureSource
    {
        private string Name;

        public abstract Capture GetCapture();

        public CaptureSource(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
