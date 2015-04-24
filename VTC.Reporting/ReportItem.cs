using System;
using System.Threading;

namespace VTC.Reporting
{
    /// <summary>
    /// Overlap Report Handling signals what to do when a long-running report 
    /// does not complete before the next report tick occurs
    /// </summary>
    public enum OverlapReportHandling
    {
        AllowOverlappedReports,
        IgnoreOverlappedReports,
        DelayOverlappedReports
    }

    public abstract class ReportItem
    {
        private int ReportIntervalMinutes;
        private OverlapReportHandling OverlapHandling;
        private Mutex AccessControl = new Mutex();

        public ReportItem(int _reportIntervalMinutes, OverlapReportHandling _overlapHandling = OverlapReportHandling.IgnoreOverlappedReports)
        {
            this.ReportIntervalMinutes = _reportIntervalMinutes;
            this.OverlapHandling = _overlapHandling;
        }

        public void ReportIfIntervalUp(long runTimeMinutes)
        {
            if (runTimeMinutes % (long)ReportIntervalMinutes == 0)
            {
                bool locked = false;

                switch (OverlapHandling)
                {
                    case OverlapReportHandling.AllowOverlappedReports:
                        // No need to touch the mutex if concurrency is allowed
                        break;
                    case OverlapReportHandling.DelayOverlappedReports:
                        // Wait for mutex control.  There is a danger here of holding up all remaining report items waiting to be processed.
                        locked = AccessControl.WaitOne();
                        if (!locked)
                        {
                            throw new Exception("Failed to obtain mutex lock");
                        }
                        break;
                    case OverlapReportHandling.IgnoreOverlappedReports:
                        locked = AccessControl.WaitOne(0);
                        if (!locked) return;
                        break;
                }
                
                // Interval hit
                Report();

                if (locked) AccessControl.ReleaseMutex();

            }
        }

        protected abstract void Report();
    }
}
