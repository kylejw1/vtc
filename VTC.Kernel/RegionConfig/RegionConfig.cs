using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace VTC.Kernel.RegionConfig
{ 
    [DataContract]
    public class RegionConfig
    {
        [DataMember]
        public Polygon RoiMask;

        [DataMember]
        public Dictionary<string, Polygon> Regions;

        [DataMember]
        public string Title { get; set; }

        public RegionConfig()
        {
            RoiMask = new Polygon();
            Regions = new Dictionary<string, Polygon>();
        }

        public RegionConfig DeepCopy()
        {
            var copy = new RegionConfig();

            foreach (var pt in RoiMask)
            {
                copy.RoiMask.Add(new Point(pt.X, pt.Y));
            }

            foreach (var kvp in Regions)
            {
                copy.Regions[kvp.Key] = new Polygon();
                foreach (var pt in kvp.Value)
                {
                    copy.Regions[kvp.Key].Add(new Point(pt.X, pt.Y));
                }

                copy.Regions[kvp.Key].Centroid = kvp.Value.Centroid;
            }

            return copy;
        }

    }
}
