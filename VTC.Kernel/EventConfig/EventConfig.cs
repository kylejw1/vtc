using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VTC.Kernel.EventConfig
{
    public struct RegionTransition
    {
        public string in_region;
        public string out_region;

        public RegionTransition(int in_region, int out_region)
        {
            this.in_region ="Approach " + in_region.ToString();
            this.out_region = "Exit " + out_region.ToString();
        }
    }

    [DataContract]
    public class EventConfig
    {
        
        [DataMember]
        public Dictionary<RegionTransition, string> Events;

        public EventConfig()
        {
            Events = new Dictionary<RegionTransition, string>();
        }

        public EventConfig DeepCopy()
        {
            var copy = new EventConfig();

            foreach (var evt in Events)
            {
                copy.Events.Add(evt.Key, evt.Value);
            }
            return copy;
        }

        public void Save(string path)
        {
            using (var f = System.IO.File.Create(path))
            {
                DataContractSerializer s = new DataContractSerializer(this.GetType());
                s.WriteObject(f, this);
            }
        }

        public static EventConfig Load(string path)
        {
            try
            {
                using (var f = System.IO.File.OpenRead(path))
                {
                    DataContractSerializer s = new DataContractSerializer(typeof(EventConfig));
                    var pgr = (EventConfig)s.ReadObject(f);

                    return pgr;
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
