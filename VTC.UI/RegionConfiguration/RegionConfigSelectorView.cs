using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    using CaptureSource = CaptureSource.CaptureSource;

    public partial class RegionConfigSelectorView : Form, IRegionConfigSelectorView
    {
        private BindingList<RegionConfig> _regionConfigs;
        private Dictionary<RegionConfigSelectorControl, CaptureSource> _captureSourceLookup = new Dictionary<RegionConfigSelectorControl, CaptureSource>();

        public RegionConfigSelectorView() 
        {
            InitializeComponent();
        }

        public event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;

        private Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl> _controlLookup = new Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl>();

        private RegionConfigSelectorControl CreateRegionConfigSelectorControl(BindingList<RegionConfig> regionConfigs, CaptureSource captureSource)
        {
            var control = new RegionConfigSelectorControl(regionConfigs, captureSource.QueryFrame().Convert<Emgu.CV.Structure.Bgr, float>(), captureSource.Name);

            control.BorderStyle = BorderStyle.FixedSingle;

            control.Width =
                tlpControls.Width - tlpControls.Padding.Left - tlpControls.Padding.Right
                - control.Margin.Left - control.Margin.Right
                - 100;

            control.Anchor =  AnchorStyles.Left | AnchorStyles.Right;

            control.CreateNewRegionConfigClicked += OnCreateNewRegionConfigClicked;

            return control;
        }

        private void OnCreateNewRegionConfigClicked(object sender, RegionConfigSelectorEventArgs e)
        {
            if (null != CreateNewRegionConfigClicked)
                CreateNewRegionConfigClicked(sender, e);
        }

        public void SetModel(RegionConfigSelectorModel model)
        {
            _regionConfigs = new BindingList<RegionConfig>(model.RegionConfigs);

            _captureSourceLookup.Clear();
            tlpControls.Controls.Clear();
            tlpControls.RowCount = 0;

            foreach (var captureSource in model.CaptureSources)
            {
                var control = CreateRegionConfigSelectorControl(_regionConfigs, captureSource);
                _captureSourceLookup[control] = captureSource;
                tlpControls.RowCount++;
                tlpControls.Controls.Add(control);
            }
        }

        public void AddRegionConfig(RegionConfig regionConfig)
        {
            _regionConfigs.Add(regionConfig);
        }
    }
}
