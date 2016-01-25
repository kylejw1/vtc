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

        public int? MaxKeyLength = 28;
        public Regex ValidCharacterRegex = new Regex(@"[\w\d]");

        public LicenseModel(Type licensedObjectType, object licensedObjectInstance, string publicKey)
        {
            try
            {
                License = ExtendedLicenseManager.GetLicense(licensedObjectType, licensedObjectInstance, publicKey);
            }
            catch (Exception ex)
            {
                var message = String.Format("Failed to retrieve license. type={0} publicKey={1}",
                    licensedObjectType, publicKey);
                throw new Exception(message, ex);
            }
        }

        public bool ValidateKeyFormat(string key, out string message)
        {
            if (Regex.IsMatch(key, @"[^\w\d-]"))
            {
                message = "Key contains invalid characters." + Environment.NewLine + "Only letters, numbers, hyphens";
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

        public bool TryActivate(string key, out string message)
        {
            try
            {
                string result = License.Activate(key);

                message = String.Empty;

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return false;
            }
        }

        // TODO: This doesnt seem to work within the IPManager
        //public void Deactivate()
        //{
        //    var result = License.Deactivate();
        //}

        public bool IsActivatedAndGenuine
        {
            get
            {
                GenuineResult result;

                try
                {
                    var active = License.Validate();
                    result = License.IsGenuineEx();
                }
                catch (Exception ex)
                {
                    throw new Exception("License Genuine check failed: " + ex.Message, ex);
                }

                var retVal = false;

                if (result == GenuineResult.InternetError)
                {
                    // TODO: Handle connection errors
                }

                return result == GenuineResult.Genuine || result == GenuineResult.InternetError;
            }
        }

    }
}
