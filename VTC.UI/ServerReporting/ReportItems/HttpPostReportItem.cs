using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using VTC.Kernel;

namespace VTC.ServerReporting.ReportItems
{
    public class HttpPostReportItem
    {

        public static void SendStatePOST(string postUrl, string postString)
        {

                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(postUrl);
                objRequest.KeepAlive = true;
                objRequest.Pipelined = true;
                objRequest.Timeout = 2000;
                objRequest.Method = "POST";
                objRequest.ContentLength = postString.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";

                //Trying different things to prevent so many timeouts:
                objRequest.KeepAlive = false;
                ServicePointManager.Expect100Continue = false;

                //// post data is sent as a stream
                using(StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
                {
                    myWriter.Write(postString);
                    myWriter.Close();
                }
                objRequest.GetResponse();
                objRequest.Abort();
            
        }

        public static string PostStateString(StateEstimate[] stateEstimates, string IntersectionID, string ServerUrl, out string postString)
        {
            Dictionary<string, string> postValues = new Dictionary<string, string>();
            for (int vehicleCount = 0; vehicleCount < stateEstimates.Length; vehicleCount++)
            {
                String x = stateEstimates[vehicleCount].x.ToString(CultureInfo.InvariantCulture);
                String y = stateEstimates[vehicleCount].y.ToString(CultureInfo.InvariantCulture);
                String vx = stateEstimates[vehicleCount].vx.ToString(CultureInfo.InvariantCulture);
                String vy = stateEstimates[vehicleCount].vy.ToString(CultureInfo.InvariantCulture);
                const string zero = "0";
                postValues.Add("state_sample[states_attributes][" + vehicleCount + "][x]", x);
                postValues.Add("state_sample[states_attributes][" + vehicleCount + "][vx]", vx);
                postValues.Add("state_sample[states_attributes][" + vehicleCount + "][y]", y);
                postValues.Add("state_sample[states_attributes][" + vehicleCount + "][vy]", vy);
                postValues.Add("state_sample[states_attributes][" + vehicleCount + "][_destroy]", zero);
            }

            if (stateEstimates.Length == 0)
                postValues.Add("state_sample[states_attributes][]", "");

            postValues.Add("intersection_id", IntersectionID);


            postString = "";
            foreach (KeyValuePair<string, string> postValue in postValues)
            {
                postString += postValue.Key + "=" + HttpUtility.UrlEncode(postValue.Value) + "&";
            }
            postString = postString.TrimEnd('&');

            //Upload state to server
            string postUrl = "http://" + ServerUrl + "/state_samples";
            return postUrl;
        }
    }
}
