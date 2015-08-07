using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace TemplateMatcherForm
{
    public partial class TemplateMatcherForm : Form
    {

        private Image<Bgra, byte>[] _templates;
 
        public TemplateMatcherForm()
        {
            InitializeComponent();
        }

        private void loadTemplatesButton_Click(object sender, EventArgs e)
        {
            var folderPath = templatesPathTextbox.Text;
            var paths = Directory.GetFiles(folderPath);
            _templates = new Image<Bgra, byte>[paths.Length];
            for (var i = 0; i < paths.Length; i++)
                  _templates[i] = new Image<Bgra, byte>(paths[i]);
        }

        private void findBestReconstructionButton_Click(object sender, EventArgs e)
        {

        }

        private void generateReconstructionButton_Click(object sender, EventArgs e)
        {
            var inputImage = new Image<Bgra, byte>(inputImagePathTextbox.Text + "\\InputSingle.png");
            var image = new Image<Bgra, byte>(inputImage.Width, inputImage.Height, new Bgra(0,0,0,0));
            var paddedImage = PaddedImage(image);
            var maxDimension = _templates.Select(template => Math.Max(template.Width, template.Height)).Concat(new[] { 0 }).Max();

            var bmp = paddedImage.Bitmap;
            var gra = Graphics.FromImage(bmp);
            gra.CompositingMode = CompositingMode.SourceOver;
            for (int i = 0; i < reconstructionDataGridview.RowCount-1; i++)
            {
                var index = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[0].Value.ToString()); 
                var template = _templates[index];
                //int x = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[1].Value.ToString()) + maxDimension;
                //int y = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[2].Value.ToString()) + maxDimension;
                int x = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[1].Value.ToString());
                int y = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[2].Value.ToString());
                paddedImage.ROI = new Rectangle(x, y, template.Width, template.Height);
                //template.CopyTo(paddedImage);
                gra.DrawImage(template.ToBitmap(), new Point(x,y));
            }

            paddedImage.ROI = new Rectangle(maxDimension, maxDimension, image.Width, image.Height);
            paddedImage.CopyTo(image);
            reconstructionPicturebox.Image = image.Bitmap;
            
            //var differenceImage = paddedImage.AbsDiff(inputImage);
            var differenceImage = inputImage.AbsDiff(paddedImage);
            reconstructionErrorPicturebox.Image = differenceImage.Bitmap;

            var reconstructionErrorSum = differenceImage.GetSum().Red + differenceImage.GetSum().Green + differenceImage.GetSum().Blue;
            reconstructionErrorTextbox.Text = reconstructionErrorSum.ToString();
        }

        private Image<Bgra, byte> PaddedImage(Image<Bgra, byte> imageIn)
        {
            var maxDimension = _templates.Select(template => Math.Max(template.Width, template.Height)).Concat(new[] {0}).Max();
            var paddedIm = new Image<Bgra, byte>(new Size(imageIn.Width + 2*maxDimension, imageIn.Height + 2*maxDimension));
            paddedIm.ROI = new Rectangle(maxDimension, maxDimension, imageIn.Width, imageIn.Height);
            imageIn.CopyTo(paddedIm);
            return paddedIm;
        }
    }
}
