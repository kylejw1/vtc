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
            if (null == model)
                throw new ArgumentNullException("model");

            if (null == view)
                throw new ArgumentNullException("view");

            _view = view;
            _model = model;

            _view.SetController(this);
            _view.SetMaxKeyLength(_model.MaxKeyLength);
            _view.SetValidCharacterRegex(_model.ValidCharacterRegex);
        }

        public void Activate()
        {
            _view.SetStatusMessage("Activating...");

            string message;
            string key = _view.GetLicenseKey();

            if (!_model.ValidateKeyFormat(key, out message))
            {
                _view.SetStatusMessage(message);
                return;
            }

            if (!_model.TryActivate(_view.GetLicenseKey(), out message))
            {
                _view.SetStatusMessage(message);
                return;
            }

            _view.SetStatusMessage(string.Empty);
            _view.ShowActivationSuccess("Thank you for activating.");

            _view.Close();
        }

        
    }
}
