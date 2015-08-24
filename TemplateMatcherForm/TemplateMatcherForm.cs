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
using TemplateMatcher;

namespace TemplateMatcherForm
{
    public partial class TemplateMatcherForm : Form
    {

        private Image<Bgra, byte>[] _templates;
        private TemplateMatcher.TemplateMatcher _tm;
 
        public TemplateMatcherForm()
        {
            InitializeComponent();
            _tm = new TemplateMatcher.TemplateMatcher();
        }

        private void loadTemplatesButton_Click(object sender, EventArgs e)
        {
            var folderPath = templatesPathTextbox.Text;
            _tm.PopulateTemplates(folderPath);
        }

        private void findBestReconstructionButton_Click(object sender, EventArgs e)
        {
            var inputImage = new Image<Bgra, byte>(inputImagePathTextbox.Text + "\\InputSingle.png");
            inputImageBox.Image = inputImage.Bitmap;
            var template = _tm.SingleBestAssignment(inputImage);

            var templateAssignments = new TemplateAssignment[1];
            templateAssignments[0] = template;

            var image = _tm.ReconstructImage(templateAssignments, inputImage.Width, inputImage.Height);
            reconstructionPicturebox.Image = image.Bitmap;
            var differenceImage = inputImage.AbsDiff(image);
            reconstructionErrorPicturebox.Image = differenceImage.Bitmap;

            var reconstructionErrorSum = _tm.EvaluateAssignment(templateAssignments, inputImage);
            reconstructionErrorTextbox.Text = reconstructionErrorSum.ToString();

            reconstructionDataGridview.RowCount = templateAssignments.Length;
            for (int i = 0; i < templateAssignments.Length; i++)
            {
                reconstructionDataGridview.Rows[i].Cells[0].Value = templateAssignments[i].Template.ToString();
                reconstructionDataGridview.Rows[i].Cells[1].Value = templateAssignments[i].XShift.ToString();
                reconstructionDataGridview.Rows[i].Cells[2].Value = templateAssignments[i].YShift.ToString();
            }
        }

        private void generateReconstructionButton_Click(object sender, EventArgs e)
        {
            var inputImage = new Image<Bgra, byte>(inputImagePathTextbox.Text + "\\InputSingle.png");
            var templateAssignments = new TemplateAssignment[reconstructionDataGridview.RowCount - 1];
            for (int i = 0; i < reconstructionDataGridview.RowCount-1; i++)
            {
                templateAssignments[i].Template = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[0].Value.ToString());
                int x = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[1].Value.ToString());
                int y = Int32.Parse(reconstructionDataGridview.Rows[i].Cells[2].Value.ToString());
                templateAssignments[i].XShift = x;
                templateAssignments[i].YShift = y;
            }

            var image = _tm.ReconstructImage(templateAssignments, inputImage.Width, inputImage.Height);
            reconstructionPicturebox.Image = image.Bitmap;
            var differenceImage = inputImage.AbsDiff(image);
            reconstructionErrorPicturebox.Image = differenceImage.Bitmap;

            var reconstructionErrorSum = _tm.EvaluateAssignment(templateAssignments, inputImage);
            reconstructionErrorTextbox.Text = reconstructionErrorSum.ToString();
        }
    }
}
