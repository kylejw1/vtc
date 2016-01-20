using SkyXoft.BusinessSolutions.LicenseManager.Protector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LicenseManager
{
    public class LicenseModel
    {
        private readonly ExtendedLicense License;

        private string _key;

        public LicenseModel(Type licensedObjectType, object licensedObjectInstance, string publicKey, string serialNumber)
        {
            try
            {
                //var info = new LicenseValidationInfo()
                //{
                //    LicenseFile = new LicenseFile("license.xml")
                //};
                //var fileLic = ExtendedLicenseManager.GetLicense(licensedObjectType, licensedObjectInstance, info, serialNumber);

                //var fileGen = fileLic.IsGenuineEx();

                //var kyle = ExtendedLicenseManager.GetLicense(licensedObjectType, licensedObjectInstance, publicKey);

                //var deact = kyle.Deactivate();
                //var gen1 = kyle.IsGenuineEx();
                //var act = kyle.Activate(serialNumber, false);
                //var gen = kyle.IsGenuineEx();
                //var gen2 = kyle.IsGenuine(false, publicKey);
                //var valid = kyle.Validate();
                //License = ExtendedLicenseManager.GetLicense(licensedObjectType, licensedObjectInstance, publicKey);
            }
            catch (Exception ex)
            {
                var message = String.Format("Failed to retrieve license. type={0} publicKey={1} serialNumber={2}",
                    licensedObjectType, publicKey, serialNumber);
                throw new Exception(message, ex);
            }

            // TODO: In controller, only activate if nothing already activated?  Prompt for deactivate
        }

        public bool ValidateFormatAndSetKey(string key, out string message)
        {
            if (Regex.IsMatch(key, @"[^\w\d-]"))
            {
                message = "Key contains invalid characters";
                return false;
            }

            if (key.Replace("-", string.Empty).Length != 28)
            {
                message = "Key must be 28 characters not including hyphens.";
                return false;
            }

            _key = key.ToUpperInvariant();
            message = String.Empty;

            return true;
        }

        public bool IsActivated
        {
            get
            {
                var result = License.IsGenuineEx();

                return result == GenuineResult.Genuine;
            }
        }

        public bool IsGenuine
        {
            get
            {
                var result = License.IsGenuineEx();

                if (result == GenuineResult.InternetError)
                {
                    throw new Exception("Cannot contact validation servers.  Please verify connectivity and try again.");
                }

                return result == GenuineResult.Genuine;
            }
        }

        public void Activate(string serialNumber, bool saveFile)
        {
            string result = License.Activate(serialNumber, saveFile);
        }

        public void Deactivate()
        {
            var result = License.Deactivate();
        }

    }
}
