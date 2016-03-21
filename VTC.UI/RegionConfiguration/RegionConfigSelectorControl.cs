using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public partial class RegionConfigSelectorControl : UserControl
    {
        public event SelectedRegionConfigChangedEventHandler SelectedRegionConfigChanged;
        public event CreateNewRegionConfigClickedEventHandler CreateNewRegionConfigClicked;

        private RegionConfigSelectorModel _model;
        public RegionConfigSelectorModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                if (null != _model)
                {
                    lblName.Text = Model.CaptureSource.Name;
                    pbThumbnail.Image = Model.Thumbnail.ToBitmap();
                    RegionConfigurations = Model.RegionConfigs;
                }
            }
        }

        public RegionConfigSelectorControl()
        {
            InitializeComponent();
        }

        private IEnumerable<RegionConfig> _regionConfigurations = new List<RegionConfig>();
        public IEnumerable<RegionConfig> RegionConfigurations
        {
            get
            {
                return _regionConfigurations;
            }
            set
            {
                var oldSelected = lbRegionConfigs.SelectedItem;

                _regionConfigurations = value;
                lbRegionConfigs.DataSource = _regionConfigurations;
                lbRegionConfigs.DisplayMember = "Title";

                if (_regionConfigurations.Contains(oldSelected))
                {
                    lbRegionConfigs.SelectedItem = oldSelected;
                }
            }
        }

        public Image Thumbnail
        {
            get
            {
                return pbThumbnail.Image;
            }
            set
            {
                pbThumbnail.Image = value;
            }
        }

        public RegionConfig SelectedRegionConfig
        {
            get { return lbRegionConfigs.SelectedItem as RegionConfig; }
            set
            {
                if (lbRegionConfigs.Items.Contains(value))
                    lbRegionConfigs.SelectedItem = value;
            }
        }

        private void lbRegionConfigs_SelectedValueChanged(object sender, EventArgs e)
        {
            var lb = sender as ListBox;
            if (null == lb)
                return;

            var item = lb.SelectedItem as RegionConfig;

            if (null != SelectedRegionConfigChanged)
                SelectedRegionConfigChanged(sender, new RegionConfigSelectorEventArgs() { Model = Model, SelectedRegionConfig = item });
        }

        private void btnCreateNewRegionConfig_Click(object sender, EventArgs e)
        {
            var lb = this.lbRegionConfigs;
            if (null == lb)
                return;

            var item = lb.SelectedItem as RegionConfig;

            if (null != CreateNewRegionConfigClicked)
                CreateNewRegionConfigClicked(sender, new RegionConfigSelectorEventArgs() { Model = Model, SelectedRegionConfig = item });
        }


    }
}
