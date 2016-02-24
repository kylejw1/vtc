using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using VTC.Common;
using Emgu.CV;


namespace VTC.Reporting
{
    public class TrajectoryLogger
    {
        private readonly ISettings _settings;

        //Data values
        private readonly string _movementType;
        private readonly List<Measurement> _measurementList;
        private readonly List<IImage> _images;
        private readonly DateTime _time;
        private readonly string _objectType;
        private readonly string _source;

        public TrajectoryLogger(ISettings settings, List<Measurement> measurements, string movementType, string source)
        {
            _settings = settings;
            _measurementList = measurements;
            _movementType = movementType;
            _time = DateTime.Now;
            _source = source;
        }

        public TrajectoryLogger(ISettings settings, string movementType, string objectType, string source)
        {
            _settings = settings;
            _movementType = movementType;
            _objectType = objectType;
            _time = DateTime.Now;

            _source = source;
        }

        public TrajectoryLogger(ISettings settings, List<Measurement> measurements, string movementType, List<IImage> images)
        {
            _settings = settings;
            _measurementList = measurements;
            _movementType = movementType;
            _images = images;
            _time = DateTime.Now;
        }

        public void LogAndPOST()
        {
            LogToTextfile();
            LogWithPOST();
        }

        public void LogToTextfile()
        {
            try
            {
                var logString = "";
                logString += _time.ToString("yyyy-MM-ddThh:mm:ssss%K ");
                logString += _objectType + " ";
                logString += _movementType + " ";

                string filename = "Movement Count " + SanitizeFilename(_source) + ".txt";
                filename = filename.Replace("file-", "");
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                if (!File.Exists(filepath))
                    File.Create(filepath);

                using(var sw = new StreamWriter(filepath, true))
                    sw.WriteLine(logString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("LogToTextfile: e.Message");
            }
            
        }

        

        public void LogWithPOST()
        {
            var success = false;
            try
            {
                // ER: TODO: most likely, it should not be in kernel

                var postValues = new Dictionary<string, string>();
                postValues.Add("event_report[intersection_id]", _settings.IntersectionID);
                postValues.Add("event_report[event_type]", "Turn");
                postValues.Add("event_report[content]", _movementType);

                var postString = postValues.Aggregate("", (current, postValue) => current + (postValue.Key + "=" + HttpUtility.UrlEncode(postValue.Value) + "&"));
                postString = postString.TrimEnd('&');

                //Upload state to server
                var postUrl = "http://" + _settings.ServerUrl + "/event_reports";

                var objRequest = (HttpWebRequest)WebRequest.Create(postUrl);
                objRequest.KeepAlive = true;
                objRequest.Pipelined = true;
                objRequest.Timeout = 10000;
                objRequest.Method = "POST";
                objRequest.ContentLength = postString.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";

                //// post data is sent as a stream
                StreamWriter myWriter = null;

                var t2 = new Thread(delegate ()
                {
                    try
                    {
                        myWriter = new StreamWriter(objRequest.GetRequestStream());
                        myWriter.Write(postString);
                        myWriter.Close();
                        objRequest.GetResponse().Close();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        //throw (ex);      
                    }
                    finally
                    {
                        Debug.WriteLine("Post turn report success: " + success + " " + DateTime.Now);
                    }
                });
                t2.Start();
            }
            catch (Exception ex)
            {
#if(DEBUG)
                Debug.WriteLine(ex.Message);
                //throw (ex);      
#else
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.InnerException);
                Trace.WriteLine(ex.StackTrace);
                Trace.WriteLine(ex.TargetSite);
            }
#endif
            }

            return;
        }


        //Taken from daniweb
        //https://www.daniweb.com/programming/software-development/threads/217968/generate-safe-filenames-with-c
        private string SanitizeFilename(string filename)
        {
            // first trim the raw string
            string safe = filename.Trim();
            // replace spaces with hyphens
            safe = safe.Replace(" ", "-").ToLower();
            // replace any 'double spaces' with singles
            if (safe.IndexOf("--") > -1)
                while (safe.IndexOf("--") > -1)
                    safe = safe.Replace("--", "-");
            // trim out illegal characters
            safe = Regex.Replace(safe, "[^a-z0-9\\-]", "");
            // trim the length
            if (safe.Length > 50)
                safe = safe.Substring(0, 49);
            // clean the beginning and end of the filename
            char[] replace = { '-', '.' };
            safe = safe.TrimStart(replace);
            safe = safe.TrimEnd(replace);
            return safe;
        }

    }
}
