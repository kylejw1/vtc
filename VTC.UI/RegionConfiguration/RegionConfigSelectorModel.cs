using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    using System.Collections.Generic;
    using CaptureSource = CaptureSource.CaptureSource;

    public class RegionConfigSelectorModel
    {
        public readonly List<CaptureSource> CaptureSources;
        public readonly List<RegionConfig> RegionConfigs;

        public RegionConfigSelectorModel(List<CaptureSource> captureSources, List<RegionConfig> regionConfigs)
        {
            CaptureSources = captureSources;
            RegionConfigs = regionConfigs;
        }
    }
}
