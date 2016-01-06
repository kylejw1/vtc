using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager
{
    public interface ILicenseManagerView
    {
        void SetController(LicenseManagerController controller);
    }
}
