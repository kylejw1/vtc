using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTC.ServerReporting
{
    public abstract class ReportItem
    {
        private int ReportIntervalMinutes;

        public ReportItem(int _reportIntervalMinutes)
        {
            this.ReportIntervalMinutes = _reportIntervalMinutes;
        }

        public void ReportIfIntervalUp(long runTimeMinutes)
        {
            if (runTimeMinutes % (long)ReportIntervalMinutes == 0)
            {
                // Interval hit
                Report();
            }
        }

        protected abstract void Report();
    }
}
