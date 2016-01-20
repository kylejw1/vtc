using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager
{
    public class LicenseManagerController
    {
        LicenseModel _model;
        ILicenseManagerView _view;

        public LicenseManagerController(LicenseModel model, ILicenseManagerView view)
        {
            _view = view;
            _model = model;

            if (null != view)
                _view.SetController(this);
        }

        public void ValidateAndSetKey(string key)
        {
            if (null == _model)
                return;

            string message;
            if (!_model.ValidateFormatAndSetKey(key, out message))
            {
                if (null != _view)
                    _view.SetKeyError(message);
            } else
            {
                _view.SetKeyError(string.Empty);
            }
        }

        
    }
}
