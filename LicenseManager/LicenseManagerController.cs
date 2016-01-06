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

            _view.SetController(this);
        }

        public void SetLicenseKey(string key)
        {
            _model.SetLicenseKey(key);

            if (_model.License.Validate())
            {

            } else
            {

            }
        }
    }
}
