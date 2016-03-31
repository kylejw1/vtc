using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    using System.Linq;
    using CaptureSource = CaptureSource.CaptureSource;

    public partial class RegionConfigSelectorView : Form, IRegionConfigSelectorView
    {
        private BindingList<RegionConfig> _regionConfigs;
        private Dictionary<RegionConfigSelectorControl, CaptureSource> _captureSourceLookup = new Dictionary<RegionConfigSelectorControl, CaptureSource>();

        public RegionConfigSelectorView() 
        {
            InitializeComponent();
        }

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

        private void OnCreateNewRegionConfigClicked(object sender, EventArgs e)
        {
            var control = sender as RegionConfigSelectorControl;

            var createRegionConfigForm = new RegionEditor(control.BaseThumbnail, new RegionConfig());
            if (createRegionConfigForm.ShowDialog() == DialogResult.OK)
            {
                var newRegionConfig = createRegionConfigForm.RegionConfig;
                AddRegionConfig(newRegionConfig);
            }
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

        private void AddRegionConfig(RegionConfig regionConfig)
        {
            // TODO: Hack until the region editor is fixed
            if (string.IsNullOrEmpty(regionConfig.Title))
            {
                regionConfig.Title = "(new regionConfig)";
            }
            _regionConfigs.Add(regionConfig);
        }

        public Dictionary<CaptureSource, RegionConfig> GetRegionConfigSelections()
        {
            var result = new Dictionary<CaptureSource, RegionConfig>();

            foreach(var kvp in _captureSourceLookup)
            {
                var captureSource = kvp.Value;
                var regionConfig = kvp.Key.SelectedRegionConfig;

                result[captureSource] = regionConfig;
            }

            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public RegionConfigSelectorModel GetModel()
        {
            return new RegionConfigSelectorModel(_captureSourceLookup.Values.ToList(), _regionConfigs.ToList());
        }
    }
}
