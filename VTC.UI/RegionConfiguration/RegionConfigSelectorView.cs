using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public partial class RegionConfigSelectorView : Form, IRegionConfigSelectorView
    {
        public RegionConfigSelectorView()
        {
            InitializeComponent();
        }

        public event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;
        public event SelectedRegionConfigChangedEventHandler SelectedRegionConfigChanged;

        private Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl> _controlLookup = new Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl>();
        
        public void AddCaptureSource(RegionConfigSelectorModel model)
        {
            var control = CreateRegionConfigSelectorControl();
            _controlLookup[model] = control;
            control.Model = model;
            tlpControls.RowCount++;
            tlpControls.Controls.Add(control);
        }

        public void UpdateCaptureSource(RegionConfigSelectorModel model, Image thumbnail, IEnumerable<RegionConfig> regionConfigs, RegionConfig selectedRegionConfig)
        {
            if (!_controlLookup.ContainsKey(model))
                return;

            if (null != thumbnail)
            {
                _controlLookup[model].Thumbnail = thumbnail;
            }

            if (null != regionConfigs)
            {
                _controlLookup[model].RegionConfigurations = regionConfigs;
            }

            if (null != selectedRegionConfig)
            {
                _controlLookup[model].SelectedRegionConfig = selectedRegionConfig;
            }
        }

        private RegionConfigSelectorControl CreateRegionConfigSelectorControl()
        {
            var control = new RegionConfigSelectorControl();

            control.BorderStyle = BorderStyle.FixedSingle;

            control.Width =
                tlpControls.Width - tlpControls.Padding.Left - tlpControls.Padding.Right
                - control.Margin.Left - control.Margin.Right
                - 100;

            control.Anchor =  AnchorStyles.Left | AnchorStyles.Right;

            control.SelectedRegionConfigChanged += OnSelectedRegionConfigChanged;
            control.CreateNewRegionConfigClicked += OnCreateNewRegionConfigClicked;

            return control;
        }

        private void OnCreateNewRegionConfigClicked(object sender, RegionConfigSelectorEventArgs e)
        {
            if (null != CreateNewRegionConfigClicked)
                CreateNewRegionConfigClicked(sender, e);
        }

        private void OnSelectedRegionConfigChanged(object sender, RegionConfigSelectorEventArgs e)
        {
            if (null != SelectedRegionConfigChanged)
                SelectedRegionConfigChanged(sender, e);
        }
    }
}
