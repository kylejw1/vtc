using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace VTC.Reporting
{
    public class ServerReporter
    {
        private Timer ReportTimer;
        private List<ReportItem> ReportItems;

        // Server reporter is a singleton as we don't want to have multiple instances all
        // reporting back on their own
        private static ServerReporter _instance = null;
        public static ServerReporter INSTANCE
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new ServerReporter();
                }

                return _instance;
            }
        }

        private ServerReporter()
        {
            ReportItems = new List<ReportItem>();
        }

        public void Start()
        {
            // Stop existing instance
            this.Stop();

            // Set up timer to tick every minute - different ReportItems can tick at different minute intervals
            // eg. every 1, 5, 10, etc minutes.  
            RunTimeMinutes = 0;
            ReportTimer = new Timer();
            ReportTimer.Interval = 60 * 1000;
            ReportTimer.Tick += ReportTimer_Tick;
            ReportTimer.Start();
        }

        public void Stop()
        {
            if (null != ReportTimer)
            {
                if (ReportTimer.Enabled)
                {
                    ReportTimer.Stop();
                }
            }
        }

        private static long RunTimeMinutes = 0;
        private void ReportTimer_Tick(object sender, EventArgs e)
        {
            RunTimeMinutes++;
            foreach (var reportItem in ReportItems)
            {
                try
                {
                    reportItem.ReportIfIntervalUp(RunTimeMinutes);
                }
                catch (Exception ex)
                {
                    #if(DEBUG)
                    {
                        throw (ex);
                    }
                    #else
                    {
                        Trace.WriteLine(ex.Message);
                        Trace.WriteLine(ex.InnerException);
                        Trace.WriteLine(ex.StackTrace);
                        Trace.WriteLine(ex.TargetSite);
                    }
                    #endif
                }
            }
        }

        public void AddReportItem(ReportItem item)
        {
            ReportItems.Add(item);
        }

        public void RemoveReportItem(ReportItem item)
        {
            ReportItems.Remove(item);
        }
    }
}
