using System;
using System.Collections.Generic;

namespace VTC.RegionConfiguration
{
    using System.Drawing;
    using Kernel.RegionConfig;
    public class RegionConfigSelectorPresenter
    {
        private IEnumerable<RegionConfigSelectorModel> _models;
        private IRegionConfigSelectorView _view;
        private IRegionConfigDataAccessLayer _regionConfigDAL;
        private List<RegionConfig> _regionConfigs;

        public RegionConfigSelectorPresenter(
            List<RegionConfig> regionConfigs,
            IEnumerable<RegionConfigSelectorModel> models, 
            IRegionConfigDataAccessLayer regionConfigDal, 
            IRegionConfigSelectorView view)
        {
            _models = models;
            _view = view;
            _regionConfigDAL = regionConfigDal;
            _regionConfigs = regionConfigs;

            InitializeView();
        }

        private void InitializeView()
        {
            _view.CreateNewRegionConfigClicked += _view_CreateNewRegionConfigClicked;
            _view.SelectedRegionConfigChanged += _view_SelectedRegionConfigChanged;

            foreach (var model in _models)
            {
                _view.AddCaptureSource(model);
            }
        }

        private void _view_SelectedRegionConfigChanged(object sender, RegionConfigSelectorEventArgs e)
        {
            Image thumbnail;
            if (null == e.SelectedRegionConfig)
            {
                thumbnail = e.Model.Thumbnail.ToBitmap();
            } else
            {
                thumbnail = CreateMaskedThumbnail(e.Model.Thumbnail, e.SelectedRegionConfig.RoiMask);
            }

            _view.UpdateCaptureSource(e.Model, thumbnail, null, null);
        }

        private void _view_CreateNewRegionConfigClicked(object sender, RegionConfigSelectorEventArgs e)
        {
            var createRegionConfigForm = new RegionEditor(e.Model.Thumbnail, new RegionConfig());
            if (createRegionConfigForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var newRegionConfig = createRegionConfigForm.RegionConfig;
                _regionConfigs.Add(newRegionConfig);
                _regionConfigDAL.SaveRegionConfigList(_regionConfigs);

                _view.UpdateCaptureSource(e.Model, null, _regionConfigs, newRegionConfig);
            }
        }

        private Image CreateMaskedThumbnail(Emgu.CV.Image<Emgu.CV.Structure.Bgr, float> background, Polygon polygon)
        {
            try
            {
                var mask = polygon.GetMask(background.Width, background.Height, new Emgu.CV.Structure.Bgr(Color.Blue));

                var maskedThumbnail = background.Add(mask).ToBitmap();

                return maskedThumbnail;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
