using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTC.Kernel;
using VTC.Kernel.Vistas;
using VTC.ServerReporting.ReportItems;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;

namespace OptAssignTest
{


    //TODO: Either start using the built-in parser or delete this DataContract
    [DataContract]
    public class JsonStateEstimate
    {
        [DataMember]
        public string vx { get; set; }

        [DataMember]
        public string vy { get; set; }

        [DataMember]
        public string x { get; set; }

        [DataMember]
        public string y { get; set; }
    }

    [TestClass]
    public class ConnectivityTest
    {
        [TestMethod]
        [Description("Send and read back state values from server in order to measure end-to-end loopback time.")]
        public void ServerLoopback()
        {

            GetSingleState(); // Warm up GetRequestStream() - JIT is slow

            DateTime testStart = DateTime.Now;
            int numReps = 10;
            int maxFailedChecks = 10;
            int failedChecks = 0;

            

            for (int i = 0; i < numReps; i++)
            {
                DateTime iterationStart = DateTime.Now;
                SendSingleVehicle(i,10);

                StateEstimate result;
                result = GetSingleState();

                while (result.x != i)
                {
                    result = GetSingleState();
                    if (failedChecks++ > maxFailedChecks)
                        break;
                }

                TimeSpan iterationSpan = DateTime.Now - iterationStart;
                Console.WriteLine("Single iteration time: " + iterationSpan);
            }

            TimeSpan loopbackTimeSpan = new TimeSpan( (DateTime.Now.Ticks - testStart.Ticks)/numReps );
            Console.WriteLine("Average loopback time: "+loopbackTimeSpan);

            //Each POST/GET cycle should take on average less than 5s
            Assert.IsTrue(loopbackTimeSpan < TimeSpan.FromSeconds(5));
        }

        private static void SendSingleVehicle(int x, int y)
        {
            TestSettings testSettings = new TestSettings();
            Vista vista = new IntersectionVista(testSettings, 640, 480);
            List<StateEstimate> stateHistory = new List<StateEstimate>();
            StateEstimate[] stateEstimates = new StateEstimate[1];
            stateEstimates[0].x = x;
            stateEstimates[0].y = y;
            string postString;

            string postUrl = VTC.ServerReporting.ReportItems.HttpPostReportItem.PostStateString(stateEstimates, "4",
                "dev.traffic-camera.com", out postString);
            VTC.ServerReporting.ReportItems.HttpPostReportItem.SendStatePOST(postUrl, postString);
        }

        private static StateEstimate GetSingleState()
        {
            string sURL;
            sURL = "http://dev.traffic-camera.com/intersections/newest/4.json";
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);
            wrGETURL.Timeout = 10000;
            
            
            StateEstimate stateEstimate = new StateEstimate();

            try
            {
                using (var wresponse = wrGETURL.GetResponse())
                {
                    using(Stream objStream = wresponse.GetResponseStream())
                    {
                        StreamReader readStream = new StreamReader(objStream, Encoding.UTF8);
                        string response = readStream.ReadToEnd();


                        //TODO: use a library, not ugly manual JSON parsing
                        string trimmedResponse = response.Replace("\"", ""); //also replace [,{,],}
                        trimmedResponse = trimmedResponse.Replace("[", "");
                        trimmedResponse = trimmedResponse.Replace("]", "");
                        trimmedResponse = trimmedResponse.Replace("{", "");
                        trimmedResponse = trimmedResponse.Replace("}", "");
                        var commaMatches = Regex.Matches(trimmedResponse, ",");
                        string xySubstring = trimmedResponse.Substring(commaMatches[5].Index);
                        var numberMatches = Regex.Matches(xySubstring, "[0-9]?[0-9].[0-9]" ); //Matches 15.6 or 2.3, etc

                        string x_string = xySubstring.Substring(numberMatches[0].Index, numberMatches[0].Length);
                        string y_string = xySubstring.Substring(numberMatches[1].Index, numberMatches[1].Length);
                        //DataContractJsonSerializer serializer =
                        //new DataContractJsonSerializer(typeof(JsonStateEstimates));
                        //if (objStream == null)
                        //    throw (new Exception("Json response is null"));

                        //JsonStateEstimates loopbackJsonStateEstimates = (JsonStateEstimates)serializer.ReadObject(objStream);
                        stateEstimate.x = Convert.ToDouble(x_string);
                        stateEstimate.y = Convert.ToDouble(y_string);

                        objStream.Close();
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception in GetSingleState: " + e);
            }

            
            wrGETURL.Abort();
            
           
            return stateEstimate;
        }
    }
}
