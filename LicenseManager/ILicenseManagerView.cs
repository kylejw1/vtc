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
        void SetKeyError(string errorMessage);
        void SetMaxKeyLength(int? keyLength);
        void SetValidCharacterRegex(Regex validCharacterRegex);
    }
}
