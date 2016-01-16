using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CUDATests
{
    public partial class Form1 : Form
    {

        [DllImport("TestLib.dll")]
        public static extern char[] DisplayHelloFromDLL();


        public Form1()
        {
            InitializeComponent();
        }

        private void callDLLButton_Click(object sender, EventArgs e)
        {
            char[] result = DisplayHelloFromDLL();
            var s = new string(result);
            outputTextbox.Text = s;
        }

    }
}
