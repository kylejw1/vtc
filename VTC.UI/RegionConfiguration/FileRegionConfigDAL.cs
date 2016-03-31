using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public class FileRegionConfigDAL : IRegionConfigDataAccessLayer
    {
        private readonly string Path;

        public FileRegionConfigDAL(string path)
        {
            Path = path;
        }

        public IEnumerable<RegionConfig> LoadRegionConfigList()
        {
            if (!File.Exists(Path))
            {
                return new List<RegionConfig>();
            }

            using (var file = File.OpenRead(Path))
            {
                DataContractSerializer s = new DataContractSerializer(typeof(IEnumerable<RegionConfig>));
                var regionConfigs = (IEnumerable<RegionConfig>)s.ReadObject(file);

                int titleNum = 1;
                foreach (var regionConfig in regionConfigs)
                {
                    foreach (var region in regionConfig.Regions)
                    {
                        if (region.Value.PolygonClosed)
                            region.Value.UpdateCentroid();
                    }

                    if (string.IsNullOrWhiteSpace(regionConfig.Title))
                    {
                        string title;
                        do
                        {
                            title = "Region Configuration " + titleNum++;
                        } while (regionConfigs.Any(rc => title.ToLowerInvariant().Equals((rc.Title ?? string.Empty).ToLowerInvariant())));
                        regionConfig.Title = title;
                    }

                    regionConfig.RoiMask.UpdateCentroid();
                }

                return regionConfigs;
            }
        }

        public void SaveRegionConfigList(IEnumerable<RegionConfig> regionConfigs)
        {
            byte[] serialized;

            // Ensure the new items can be serialized successfully before deleting the old file
            using (var stream = new MemoryStream())
            {
                DataContractSerializer s = new DataContractSerializer(typeof(IEnumerable<RegionConfig>));
                s.WriteObject(stream, regionConfigs);

                serialized = stream.ToArray();
            }

            File.WriteAllBytes(Path, serialized);
        }
    }
}
