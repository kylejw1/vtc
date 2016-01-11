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
        private readonly ExtendedLicense License;

        public LicenseManagerModel(Type licensedObjectType, object licensedObjectInstance, string publicKey, string serialNumber)
        {
            try
            {
                License = ExtendedLicenseManager.GetLicense(licensedObjectType, licensedObjectInstance, publicKey);
            }
            catch (Exception ex)
            {
                throw new Exception("License validation failed: " + ex.Message);
            }

            // TODO: In controller, only activate if nothing already activated?  Prompt for deactivate
        }

        public void CheckSerialNumber(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                throw new ArgumentNullException("Serial number cannot be empty.");
            }

            GenuineResult result = License.IsGenuineEx();

            bool activated = result.Equals(GenuineResult.Genuine);
        }

    }
}
