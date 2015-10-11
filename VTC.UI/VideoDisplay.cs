using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VTC
{
    public partial class VideoDisplay : Form
    {

        public string LayerName;
		public Emgu.CV.UI.ImageBox ImageBox {  get { return imageBox; } }

        public void Update(Image<Bgr, Byte> frame)
        {
            imageBox.Image = frame;
        }

        public void Update(Image<Bgr, float> frame)
        {
            imageBox.Image = frame;
        }
           

        public VideoDisplay(string name, Point initialPosition)
        {
            InitializeComponent();
			LayerName = name;
            this.Text = "VideoDisplay: " + name;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = initialPosition;
        }

        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            var relativePoint = this.PointToClient(Cursor.Position);
            xyLabel.Text = relativePoint.X.ToString() + "," + relativePoint.Y.ToString();
        }
    }
}
