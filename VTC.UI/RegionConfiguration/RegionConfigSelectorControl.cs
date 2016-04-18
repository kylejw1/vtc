using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public partial class RegionConfigSelectorControl : UserControl
    {
        public event EventHandler CreateNewRegionConfigClicked;

        public Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> BaseThumbnail
        {
            get; private set;
        }

        public RegionConfigSelectorControl(List<RegionConfig> regionConfigs, Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> baseThumbnail, string name)
        {
            InitializeComponent();

            BaseThumbnail = baseThumbnail;
            pbThumbnail.Image = BaseThumbnail.ToBitmap();

            lbRegionConfigs.DataSource = regionConfigs;
            lbRegionConfigs.DisplayMember = "Title";
            lblName.Text = name;
        }

        public void UpdateRegionConfigs(List<RegionConfig> newRegionConfigs)
        {
            var selectedTitle = SelectedRegionConfig.Title;

            lbRegionConfigs.DataSource = newRegionConfigs;

            var selected = newRegionConfigs.FirstOrDefault(r => r.Title.Equals(selectedTitle));
            if (null != selected)
            {
                lbRegionConfigs.SelectedItem = selected;
            }
        }
        
        public RegionConfig SelectedRegionConfig
        {
            get { return lbRegionConfigs.SelectedItem as RegionConfig; }
        }

        private void lbRegionConfigs_SelectedValueChanged(object sender, EventArgs e)
        {
            var regionConfig = lbRegionConfigs.SelectedItem as RegionConfig;

            var maskedThumbnail = regionConfig.RoiMask.GetMask(BaseThumbnail.Width, BaseThumbnail.Height, new Emgu.CV.Structure.Bgr(Color.Blue));

            pbThumbnail.Image = BaseThumbnail.Add(maskedThumbnail).ToBitmap();
        }

        private void btnCreateNewRegionConfig_Click(object sender, EventArgs e)
        {
            if (null != CreateNewRegionConfigClicked)
                CreateNewRegionConfigClicked(this, EventArgs.Empty);
        }


    }
}
