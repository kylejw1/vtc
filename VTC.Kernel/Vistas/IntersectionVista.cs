using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using VTC.Common;
using VTC.Kernel.EventConfig;
using VTC.Kernel.RegionConfig;
using VTC.Reporting;

namespace VTC.Kernel.Vistas
{
    public class IntersectionVista : Vista
    {
        private static readonly string ApproachText = "Approach";
        private static readonly string ExitText = "Exit";
        

        private Dictionary<string, long> TurnStats = new Dictionary<string, long>();

        public IntersectionVista(ISettings settings, int Width, int Height, RegionConfig.RegionConfig regionConfig)
            : base(settings, Width, Height, regionConfig)
        {
            for (int i = 1; i <= 4; i++) {
                var approachName = ApproachName(i);
                var exitName = ExitName(i);

                if (!this.RegionConfiguration.Regions.ContainsKey(approachName))
                    this.RegionConfiguration.Regions.Add(approachName, new Polygon());

                if (!this.RegionConfiguration.Regions.ContainsKey(exitName))
                    this.RegionConfiguration.Regions.Add(exitName, new Polygon());
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
                var startState = d.StateHistory.First();
                var startPoint = new Point((int)startState.X, (int)startState.Y);
                var startRegion = RegionConfiguration.Regions.OrderBy(r => Math.Sqrt(Math.Pow(startPoint.X - r.Value.Centroid.X, 2) + Math.Pow(startPoint.Y - r.Value.Centroid.Y, 2))).FirstOrDefault();
                if (null == startRegion.Key) continue;

                var endState = d.StateHistory.Last();
                var endPoint = new Point((int)endState.X, (int)endState.Y);
                var endRegion = RegionConfiguration.Regions.OrderBy(r => Math.Sqrt(Math.Pow(endPoint.X - r.Value.Centroid.X, 2) + Math.Pow(endPoint.Y - r.Value.Centroid.Y, 2))).FirstOrDefault();
                if (null == endRegion.Key) continue;

                //Compare this event transition against mappings from event-transitions to turn types
                var transitionEvent = EventConfiguration.Events.FirstOrDefault(kvp => kvp.Key.in_region == startRegion.Key && kvp.Key.out_region == endRegion.Key);
                if (transitionEvent.Value == null) continue;

                string turnString = startRegion.Key + " to " + endRegion.Key;
                if (!TurnStats.ContainsKey(turnString)) TurnStats[turnString] = 0;
                TurnStats[turnString]++;

                var tl = new TrajectoryLogger(Settings, transitionEvent.Value, "vehicle", GetCameraSource.Invoke());
                string filename = "Movement Count " + TrajectoryLogger.SanitizeFilename(GetCameraSource.Invoke());
                filename = filename.Replace("file-", "");
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                tl.LogAndPOST(filepath);
            }
        }

        public override void ResetStats()
        {
            TurnStats.Clear();
            base.ResetStats();
        }

        public override string GetStatString() 
        {
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine(base.GetStatString());
            int totalObjects = 0;
            foreach (var kvp in TurnStats) {
                sb.AppendLine(kvp.Key + ":  " + kvp.Value);
                totalObjects += (int) kvp.Value;
            }

            sb.AppendLine("");
            sb.AppendLine("Total objects counted: " + totalObjects);
            sb.AppendLine("Current objects tracked: " + CurrentVehicles.Count);

            return sb.ToString();
        }
    }
}
