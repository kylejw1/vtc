using SkyXoft.BusinessSolutions.LicenseManager.Protector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager
{
    public class LicenseManagerModel
    {
        public ExtendedLicense License { get; private set; }

        public void SetLicenseKey(string key)
        {
            License = ExtendedLicenseManager.GetLicense(key);
        }

    }
}
