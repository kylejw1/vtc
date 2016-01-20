using SkyXoft.BusinessSolutions.LicenseManager.Protector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseManager
{
    public partial class LicenseManagerView : Form, ILicenseManagerView
    {
        private LicenseManagerController _controller;

        public LicenseManagerView()
        {
            InitializeComponent();
        }

        public void SetController(LicenseManagerController controller)
        {
            _controller = controller;
        }

        public void SetKeyError(string errorMessage)
        {
            lblKeyError.Text = errorMessage;
        }

        private void tbKey_TextChanged(object sender, EventArgs e)
        {
            if (null != _controller)
                _controller.ValidateAndSetKey(tbKey.Text);
        }

        private void tbKey_KeyDown(object sender, KeyEventArgs e)
        {

        }
        int i = 0;
        private void tbKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            //TODO: Handle paste case specially

            if (char.IsControl(e.KeyChar))
                return;

            if (Regex.IsMatch(e.KeyChar.ToString(), @"[\w\d]"))
            {
                tbKey.AppendText(e.KeyChar.ToString());
                //var key = tbKey.Text + e.KeyChar;
                //key = Regex.Replace(key, @".{4}(?!$)", "$0-");
                //tbKey.Text = key;
            }

            e.Handled = true;
        }
    }
}
