using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTC.RegionConfiguration
{
    public partial class InputPrompt : Form
    {
        public string InputString
        {
            get
            {
                return tbInput.Text;
            }
        }

        public InputPrompt(string caption, string message)
        {
            InitializeComponent();

            this.Text = caption;
            this.lblMessage.Text = message;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
