using System;
using System.Collections.Generic;
using System.Drawing;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    // Event Delegates
    public delegate void CreateNewRegionConfigClickedEventHandler(object sender, RegionConfigSelectorEventArgs e);

    public class RegionConfigSelectorEventArgs : EventArgs
    {
        public Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> Thumbnail { get; set; }
    }

    public interface IRegionConfigSelectorView
    {
        event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;
        void SetModel(RegionConfigSelectorModel model);
        void AddRegionConfig(RegionConfig regionConfig);
    }
}
