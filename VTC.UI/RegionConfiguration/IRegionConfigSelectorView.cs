using System;
using System.Collections.Generic;
using System.Drawing;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    // Event Delegates
    public delegate void SelectedRegionConfigChangedEventHandler(object sender, SelectedRegionConfigChangedEventArgs e);
    public delegate void CreateNewRegionConfigClickedEventHandler(object sender, EventArgs e);

    public class SelectedRegionConfigChangedEventArgs : EventArgs
    {
        public RegionConfigSelectorModel Model { get; set; }
        public RegionConfig SelectedRegionConfig { get; set; }
    }

    public interface IRegionConfigSelectorView
    {
        void AddCaptureSource(RegionConfigSelectorModel model);
        void UpdateCaptureSource(RegionConfigSelectorModel model, Image thumbnail, IEnumerable<RegionConfig> regionConfigs);
        event SelectedRegionConfigChangedEventHandler SelectedRegionConfigChanged;
        event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;
    }
}
