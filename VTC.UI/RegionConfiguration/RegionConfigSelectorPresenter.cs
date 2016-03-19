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

        public RegionConfigSelectorPresenter(IEnumerable<RegionConfigSelectorModel> models, IRegionConfigSelectorView view)
        {
            _models = models;
            _view = view;

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

        private void _view_SelectedRegionConfigChanged(object sender, SelectedRegionConfigChangedEventArgs e)
        {
            var maskedThumbnail = CreateMaskedThumbnail(e.Model.Thumbnail, e.SelectedRegionConfig.RoiMask);
            _view.UpdateCaptureSource(e.Model, maskedThumbnail, null);
        }

        private void _view_CreateNewRegionConfigClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
