using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VTC
{ 
    public class ApproachExit
    {
        public Polygon Approach = new Polygon();
        public Polygon Exit = new Polygon();

        public ApproachExit() { }
    }

    public class RegionConfig
    {
        public Polygon RoiMask;
        public List<ApproachExit> ApproachExits;

        public RegionConfig(int numApproachExits)
        {
            RoiMask = new Polygon();
            ApproachExits = new List<ApproachExit>();

            for (int i = 0; i < numApproachExits; i++)
            {
                ApproachExits.Add(new ApproachExit());
            }
        }

        public RegionConfig() : this(4)
        {

        }

        public void Save(string path)
        {
            using (var f = System.IO.File.Create(path))
            {
                DataContractSerializer s = new DataContractSerializer(this.GetType());
                s.WriteObject(f, this);
            }
        }

        public static RegionConfig Load(string path)
        {
            try
            {
                using (var f = System.IO.File.OpenRead(path))
                {
                    DataContractSerializer s = new DataContractSerializer(typeof(RegionConfig));
                    var pgr = (RegionConfig)s.ReadObject(f);

                    return pgr;
                }
            }
            catch
            {
                return new RegionConfig();
            }
        }

    }
}
