using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VTC
{
    public partial class SplashScreen : Form
    {
        private readonly DateTime startTime;
        private readonly int DelayMs;
        public SplashScreen(int ms)
        {
            InitializeComponent();
            this.BringToFront();
            startTime = DateTime.Now;
            DelayMs = ms;
        }

        private void killTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - startTime > TimeSpan.FromMilliseconds(DelayMs))
                this.Close();
            else
                this.BringToFront();   
        }
    }
}
