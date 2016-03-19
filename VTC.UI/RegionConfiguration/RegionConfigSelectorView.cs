using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void SetData(IEnumerable<CaptureSource.CaptureSource> captureSources, IEnumerable<RegionConfig> regionConfigs)
        {
            var controls = captureSources.Select(cs => CreateRegionConfigSelectorControl(cs, regionConfigs));
            tlpControls.Controls.Clear();
            tlpControls.RowCount = captureSources.Count();
            tlpControls.Controls.AddRange(controls.ToArray());
        }

        private RegionConfigSelectorControl CreateRegionConfigSelectorControl(CaptureSource.CaptureSource captureSource, IEnumerable<RegionConfig> regionConfigs)
        {
            var control = new RegionConfigSelectorControl()
            {
                CaptureSource=captureSource,
                RegionConfigurations=regionConfigs
            };

            control.BorderStyle = BorderStyle.FixedSingle;

            control.Width =
                tlpControls.Width - tlpControls.Padding.Left - tlpControls.Padding.Right
                - control.Margin.Left - control.Margin.Right
                - 100;

            control.Anchor =  AnchorStyles.Left | AnchorStyles.Right;

            return control;
        }
    }
}
