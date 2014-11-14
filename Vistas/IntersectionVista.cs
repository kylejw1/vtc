using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TreeLib;

namespace VTC
{
    public class IntersectionVista : Vista
    {
        private static readonly string ApproachText = "Approach";
        private static readonly string ExitText = "Exit";

        private Dictionary<string, long> TurnStats = new Dictionary<string, long>();

        public IntersectionVista(int Width, int Height)
            : base(Width, Height)
        {
            for (int i = 1; i <= 4; i++) {
                this.RegionConfiguration.Regions.Add(ApproachName(i), new Polygon());
                this.RegionConfiguration.Regions.Add(ExitName(i), new Polygon());
            }
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
                var startCoord = d.state_history.First().coordinates;
                var startPoint = new Point((int)startCoord.x, (int)startCoord.y);

                var startRegion = this.RegionConfiguration.Regions.FirstOrDefault(r => 
                    {
                        if (!r.Key.Contains(ApproachText)) return false;

                        return startPoint.PolygonEnclosesPoint(r.Value);
                    });

                if (null == startRegion.Key) continue;

                var endCoord = d.state_history.Last().coordinates;
                var endPoint = new Point((int)endCoord.x, (int)endCoord.y);

                var endRegion = this.RegionConfiguration.Regions.FirstOrDefault(r =>
                {
                    if (!r.Key.Contains(ExitText)) return false;

                    return endPoint.PolygonEnclosesPoint(r.Value);
                });

                if (null == endRegion.Key) continue;

                var turnText = startRegion.Key + " -> " + endRegion.Key;
                if(!TurnStats.ContainsKey(turnText)) TurnStats[turnText] = 0;
                TurnStats[turnText]++; 
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

            sb.AppendLine(base.GetStatString());
            sb.AppendLine("Turn stats:");
            foreach (var kvp in TurnStats) {
                sb.AppendLine(kvp.Key + ":  " + kvp.Value);
            }

            return sb.ToString();
        }
    }
}
