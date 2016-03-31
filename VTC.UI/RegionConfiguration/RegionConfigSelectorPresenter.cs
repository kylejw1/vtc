using System;
using System.Collections.Generic;

namespace VTC.RegionConfiguration
{
    using System.Drawing;
    using Kernel.RegionConfig;
    using System.ComponentModel;
    public class RegionConfigSelectorPresenter
    {
        private RegionConfigSelectorModel _model;
        private IRegionConfigSelectorView _view;

        public RegionConfigSelectorPresenter(RegionConfigSelectorModel model, IRegionConfigSelectorView view)
        {
            _view = view;
            _model = model;

            InitializeView();
        }

        private void InitializeView()
        {
            _view.SetModel(_model);
            _view.CreateNewRegionConfigClicked += _view_CreateNewRegionConfigClicked;
        }

        private void _view_CreateNewRegionConfigClicked(object sender, RegionConfigSelectorEventArgs e)
        {
            var view = sender as RegionConfigSelectorView;

            var createRegionConfigForm = new RegionEditor(e.Thumbnail, new RegionConfig());
            if (createRegionConfigForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var newRegionConfig = createRegionConfigForm.RegionConfig;
                _view.AddRegionConfig(newRegionConfig);
            }
        }
    }
}
