using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    using Kernel.Video;
    using System.Linq;

    public partial class RegionConfigSelectorView : Form, IRegionConfigSelectorView
    {
        private List<RegionConfig> _regionConfigs;
        private Dictionary<RegionConfigSelectorControl, ICaptureSource> _captureSourceLookup = new Dictionary<RegionConfigSelectorControl, ICaptureSource>();
        private IRegionConfigDataAccessLayer _regionConfigDal;

        public RegionConfigSelectorView(IRegionConfigDataAccessLayer regionConfigDal) 
        {
            InitializeComponent();

            _regionConfigDal = regionConfigDal;
        }

        private Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl> _controlLookup = new Dictionary<RegionConfigSelectorModel, RegionConfigSelectorControl>();

        private RegionConfigSelectorControl CreateRegionConfigSelectorControl(List<RegionConfig> regionConfigs, ICaptureSource captureSource)
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

            var captureSourceList = new List<ICaptureSource>();
            captureSourceList.Add(_captureSourceLookup[control]);

            var createRegionConfigForm = new RegionEditor(captureSourceList, _regionConfigDal);
            if (createRegionConfigForm.ShowDialog() == DialogResult.OK)
            {
                _regionConfigs = _regionConfigDal.LoadRegionConfigList().ToList();
                foreach(var c in _captureSourceLookup.Keys)
                {
                    c.UpdateRegionConfigs(_regionConfigs);
                }
            }
        }

        public void SetModel(RegionConfigSelectorModel model)
        {
            _regionConfigs = model.RegionConfigs;

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

        public Dictionary<ICaptureSource, RegionConfig> GetRegionConfigSelections()
        {
            var result = new Dictionary<ICaptureSource, RegionConfig>();

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
