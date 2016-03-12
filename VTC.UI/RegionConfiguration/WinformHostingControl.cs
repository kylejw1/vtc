using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VTC.RegionConfiguration
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    [DesignerSerializer("System.ComponentModel.Design.Serialization.TypeCodeDomSerializer , System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
    public class WinformHostingControl : System.Windows.Forms.Integration.ElementHost
    {
        public WinformHostingControl(UserControl hostedControl)
        {
            base.Child = hostedControl;
            base.Size = new System.Drawing.Size((int)hostedControl.ActualWidth, (int)hostedControl.ActualHeight);
        }
    }
}
