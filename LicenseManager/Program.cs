using SkyXoft.BusinessSolutions.LicenseManager.Protector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var model = new LicenseModel(typeof(object), new object(), "", "");
            var view = new LicenseManagerView();
            var controller = new LicenseManagerController(model, view);

            Application.Run(view);
        }
    }
}
