using System.Collections.Generic;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public interface IRegionConfigSelectorView
    {
        void SetModel(RegionConfigSelectorModel model);
        RegionConfigSelectorModel GetModel();
        Dictionary<CaptureSource.CaptureSource, RegionConfig> GetRegionConfigSelections();
    }
}
