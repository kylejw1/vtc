using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LicenseManager
{
    public class LicensedFormContext : ApplicationContext
    {
        public LicensedFormContext(string publicKey, Func<bool, string, Form> licensedFormCreator, string licensedFormArguments)
        {
            var model = new LicenseModel(typeof(LicensedFormContext), this, publicKey);

            if (!model.IsActivatedAndGenuine)
            {
                var view = new LicenseManagerView();
                var controller = new LicenseManagerController(model, view);

                view.ShowDialog();
            }

            MainForm = licensedFormCreator(model.IsActivatedAndGenuine, licensedFormArguments);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitThread();
        }
    }
}
