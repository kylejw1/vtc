using System.Collections.Generic;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    using CaptureSource = CaptureSource.CaptureSource;

    public class RegionConfigSelectorModel
    {
        public CaptureSource CaptureSource;
        public Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> Thumbnail;
        public IEnumerable<RegionConfig> RegionConfigs;
    }
}
