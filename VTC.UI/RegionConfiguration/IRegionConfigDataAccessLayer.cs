using System.Collections.Generic;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public interface IRegionConfigDataAccessLayer
    {
        IEnumerable<RegionConfig> LoadRegionConfigList();
        void SaveRegionConfigList(IEnumerable<RegionConfig> regionConfigs);
    }
}
