using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TreeLib;
using VTC.Settings;

namespace VTC
{
    public class IntersectionVista : Vista
    {
        private static readonly string ApproachText = "Approach";
        private static readonly string ExitText = "Exit";

        private Dictionary<string, long> TurnStats = new Dictionary<string, long>();

        public IntersectionVista(ISettings settings, int Width, int Height)
            : base(settings, Width, Height)
        {
            for (int i = 1; i <= 4; i++) {
                this.RegionConfiguration.Regions.Add(ApproachName(i), new Polygon());
                this.RegionConfiguration.Regions.Add(ExitName(i), new Polygon());
            }


            //Hard-coded configuration for a standard intersection. TODO: Load configuration from eventConfig.xml 
            this.EventConfiguration.Events.Add(new RegionTransition(1, 1), "straight");
            this.EventConfiguration.Events.Add(new RegionTransition(2, 2), "straight");
            this.EventConfiguration.Events.Add(new RegionTransition(3, 3), "straight");
            this.EventConfiguration.Events.Add(new RegionTransition(4, 4), "straight");

            this.EventConfiguration.Events.Add(new RegionTransition(1, 2), "left");
            this.EventConfiguration.Events.Add(new RegionTransition(2, 3), "left");
            this.EventConfiguration.Events.Add(new RegionTransition(3, 4), "left");
            this.EventConfiguration.Events.Add(new RegionTransition(4, 1), "left");

            this.EventConfiguration.Events.Add(new RegionTransition(1, 4), "right");
            this.EventConfiguration.Events.Add(new RegionTransition(2, 1), "right");
            this.EventConfiguration.Events.Add(new RegionTransition(3, 2), "right");
            this.EventConfiguration.Events.Add(new RegionTransition(4, 3), "right");
        }

        private string ApproachName(int number)
        {
            return ApproachText + " " + number;
        }

        private string ExitName(int number)
        {
            return ExitText + " " + number;
        }

        protected override void UpdateChildClassStats(List<Vehicle> deleted)
        {
            foreach (var d in deleted)
            {
                var startState = d.state_history.First();
                var startPoint = new Point((int)startState.x, (int)startState.y);

                var startRegion = this.RegionConfiguration.Regions.FirstOrDefault(r => 
                    {
                        if (!r.Key.Contains(ApproachText)) return false;

                        return startPoint.PolygonEnclosesPoint(r.Value);
                    });

                if (null == startRegion.Key) continue;

                var endState = d.state_history.Last();
                var endPoint = new Point((int)endState.x, (int)endState.y);

                var endRegion = this.RegionConfiguration.Regions.FirstOrDefault(r =>
                {
                    if (!r.Key.Contains(ExitText)) return false;

                    return endPoint.PolygonEnclosesPoint(r.Value);
                });

                if (null == endRegion.Key) continue;

                //Compare this event transition against mappings from event-transitions to turn types
                var transition_event = EventConfiguration.Events.FirstOrDefault(kvp =>
                    {
                        if (kvp.Key.in_region == startRegion.Key && kvp.Key.out_region == endRegion.Key)
                            return true;
                        else
                            return false;
                    });

                if (transition_event.Value != null)
                {
                    if (!TurnStats.ContainsKey(transition_event.Value)) TurnStats[transition_event.Value] = 0;
                    TurnStats[transition_event.Value]++;

                    //POST a new EventReport here
                    postTurnReport(transition_event.Value);
                }
            }
        }

        private bool postTurnReport(string content)
        {
            try
            {
                Dictionary<string, string> post_values = new Dictionary<string, string>();
                post_values.Add("event_report[intersection_id]", Settings.IntersectionID);
                post_values.Add("event_report[event_type]", "Turn");
                post_values.Add("event_report[content]", content);

                String post_string = "";
                foreach (KeyValuePair<string, string> post_value in post_values)
                {
                    post_string += post_value.Key + "=" + HttpUtility.UrlEncode(post_value.Value) + "&";
                }
                post_string = post_string.TrimEnd('&');

                //Upload state to server
                String post_url = "http://" + Settings.ServerUrl + "/event_reports";

                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
                objRequest.KeepAlive = true;
                objRequest.Pipelined = true;
                objRequest.Timeout = 2000;
                objRequest.Method = "POST";
                objRequest.ContentLength = post_string.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";

                //// post data is sent as a stream
                StreamWriter myWriter = null;
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(post_string);
                myWriter.Close();
                objRequest.GetResponse();

            }
            catch (Exception ex)
            {
#if(DEBUG)
                {
                    Console.WriteLine(ex.Message);
                    //throw (ex);
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

            return true;
        }

        public override void ResetStats()
        {
            TurnStats.Clear();
            base.ResetStats();
        }

        public override string GetStatString() 
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(base.GetStatString());
            sb.AppendLine("Turn stats:");
            foreach (var kvp in TurnStats) {
                sb.AppendLine(kvp.Key + ":  " + kvp.Value);
            }

            return sb.ToString();
        }
    }
}
