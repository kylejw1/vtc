using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public interface IRegionConfigDataAccessLayer
    {
        IEnumerable<RegionConfig> LoadRegionConfigList();
        void SaveRegionConfigList(IEnumerable<RegionConfig> regionConfigs);
    }
}
