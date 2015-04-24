using System;
using System.Net;

namespace VTC.Reporting.ReportItems
{
    public class FtpSendFileReportItem : ReportItem
    {
        private Func<Byte[]> GetData;
        private Uri DestinationFilePath;
        private ICredentials Credentials;

        public FtpSendFileReportItem(int reportIntervalMinutes, Uri destPath, ICredentials credentials, Func<Byte[]> getData)
            : base(reportIntervalMinutes)
        {
            GetData = getData;
            DestinationFilePath = destPath;
            Credentials = credentials;
        }

        protected override void Report()
        {
            if (null == GetData) return;

            var data = GetData();

            var wc = new WebClient();
            wc.Credentials = Credentials;
            wc.UploadDataCompleted += wc_UploadDataCompleted;

            wc.UploadDataAsync(DestinationFilePath, data);

        }

        void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            bool finished = true;
        }
    }
}
