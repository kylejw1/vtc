using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;
using VTC.CaptureSource;

namespace VTC.RegionConfiguration
{
    public partial class RegionConfigSelectorControl : UserControl
    {
        private static readonly int ThumbnailWidth = 150;
        private static readonly int ThumbnailHeight = 100;

        private Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> _thumbnail;

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
                _regionConfigurations = value;
                lbRegionConfigs.DataSource = _regionConfigurations;
                lbRegionConfigs.DisplayMember = "Title";
            }
        }

        private CaptureSource.CaptureSource _captureSource;
        public CaptureSource.CaptureSource CaptureSource
        {
            get
            {
                return _captureSource;
            } 
            set
            {
                // TODO: Draw regionconfig
                _captureSource = value;
                _thumbnail = _captureSource == null ? null : _captureSource.QueryFrame().Convert<Emgu.CV.Structure.Bgr, float>().Resize(ThumbnailWidth, ThumbnailHeight, Emgu.CV.CvEnum.Inter.Area);
                lblName.Text = _captureSource == null ? string.Empty : _captureSource.Name;
                UpdateThumbnailImageWithROI();
            }
        }

        public RegionConfig SelectedRegionConfig
        {
            get { return lbRegionConfigs.SelectedItem as RegionConfig; }
        }

        private void UpdateThumbnailImageWithROI()
        {
            var region = lbRegionConfigs.SelectedItem as RegionConfig;
            if (null == region)
            {
                pbThumbnail.Image = _thumbnail.ToBitmap(ThumbnailWidth, ThumbnailHeight);
                return;
            }

            var mask = region.RoiMask.GetMask(_captureSource.Width, _captureSource.Height, new Emgu.CV.Structure.Bgr(Color.Blue));
            mask = mask.Resize(ThumbnailWidth, ThumbnailHeight, Emgu.CV.CvEnum.Inter.Area);
            try {
                pbThumbnail.Image = _thumbnail.Add(mask).ToBitmap(ThumbnailWidth, ThumbnailHeight);
            } catch (Exception ex)
            {

            }
        }

        private void lbRegionConfigs_SelectedValueChanged(object sender, EventArgs e)
        {
            var lb = sender as ListBox;
            var item = lb.SelectedItem as RegionConfig;

            UpdateThumbnailImageWithROI();
        }
    }
}
