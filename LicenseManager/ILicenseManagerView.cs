using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LicenseManager
{
    public interface ILicenseManagerView
    {
        void SetController(LicenseManagerController controller);
        void SetStatusMessage(string errorMessage);
        void SetMaxKeyLength(int? keyLength);
        void SetValidCharacterRegex(Regex validCharacterRegex);
        void ActivateLicense();
        string GetLicenseKey();
        void ShowActivationSuccess(string message);
        void Close();
    }
}
