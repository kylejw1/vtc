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
        public event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;

        private Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> _baseThumbnail;

        public RegionConfigSelectorControl(BindingList<RegionConfig> regionConfigs, Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> baseThumbnail, string name)
        {
            InitializeComponent();

            _baseThumbnail = baseThumbnail;
            pbThumbnail.Image = _baseThumbnail.ToBitmap();

            lbRegionConfigs.DataSource = regionConfigs;
            lbRegionConfigs.DisplayMember = "Title";
            lblName.Text = name;
        }

        public Image Thumbnail
        {
            get { return pbThumbnail.Image; }
            set { pbThumbnail.Image = value; }
        }
        
        public RegionConfig SelectedRegionConfig
        {
            get { return lbRegionConfigs.SelectedItem as RegionConfig; }
        }

        private void lbRegionConfigs_SelectedValueChanged(object sender, EventArgs e)
        {
            var regionConfig = lbRegionConfigs.SelectedItem as RegionConfig;

            var maskedThumbnail = regionConfig.RoiMask.GetMask(_baseThumbnail.Width, _baseThumbnail.Height, new Emgu.CV.Structure.Bgr(Color.Blue));

            pbThumbnail.Image = _baseThumbnail.Add(maskedThumbnail).ToBitmap();
        }

        private void btnCreateNewRegionConfig_Click(object sender, EventArgs e)
        {
            if (null != CreateNewRegionConfigClicked)
                CreateNewRegionConfigClicked(this, new RegionConfigSelectorEventArgs { Thumbnail = _baseThumbnail });
        }


    }
}
