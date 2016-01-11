using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager
{
    public class LicenseManagerController
    {
        LicenseManagerModel _model;
        ILicenseManagerView _view;

        public LicenseManagerController(LicenseManagerModel model, ILicenseManagerView view)
        {
            _view = view;
            _model = model;

            if (null != view)
                _view.SetController(this);
        }

        public bool TrySetLicenseKey(string key)
        {
            _model.SetLicenseKey(key);

            return _model.License.Validate();
        }
    }
}
