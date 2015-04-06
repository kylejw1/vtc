using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV.Structure;

namespace ViolaJonesTrainer
{
    public partial class ViolaJonesForm : Form
    {

        List<string> ExamplePaths = new List<string>();
        List<string> _positiveExamplePaths;
        List<string> _negativeExamplePaths;

        public ViolaJonesForm()
        {
            InitializeComponent();

            //Find example directories
            string examplesDirectory = Directory.GetCurrentDirectory() + "\\examples";
            Boolean examplesDirectoryExists = System.IO.Directory.Exists(examplesDirectory);
            if (examplesDirectoryExists)
                foreach (string classString in System.IO.Directory.GetDirectories(examplesDirectory))
                {
                    ExamplePaths.Add(classString);
                    string[] pathElements = classString.Split('\\');
                    classesComboBox.Items.Add(pathElements[pathElements.Length-1]); 
                }

            classesComboBox.SelectedItem = classesComboBox.Items[0];
            

        }

        private void classesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox) sender;
            string selectedClass = (string) cb.Items[cb.SelectedIndex];
            //Load positive examples path
            string positiveExampleRootPath = ExamplePaths.Where(path => path.Contains(selectedClass)).FirstOrDefault<string>(); ;
            _positiveExamplePaths = System.IO.Directory.GetFiles(positiveExampleRootPath).ToList<string>();

            //Display subset of positive examples
            int maxDisplayedExamples = 5;
            int displayedExamples = 0;
            int lastExampleXlocation = 10;

            positiveExampleImages.Controls.Clear();
            foreach(string examplePath in _positiveExamplePaths)
            {
                
                if(displayedExamples++ < maxDisplayedExamples)
                {
                    PictureBox exampleBox = new PictureBox();
                    Emgu.CV.Image<Gray, Byte> exampleImage = new Emgu.CV.Image<Bgr, int>(examplePath).Convert<Gray,Byte>();
                    lastExampleXlocation = lastExampleXlocation + exampleBox.Width + 5;
                    exampleBox.SetBounds(lastExampleXlocation, 10, exampleImage.Width, exampleImage.Height);
                    exampleBox.Image = exampleImage.ToBitmap();
                    positiveExampleImages.Controls.Add(exampleBox);
                }
                
            }
            

            //Load paths of negative examples
            _negativeExamplePaths = new List<string>();
            string[] negativeExampleRootPaths = ExamplePaths.Where(path => !path.Contains(selectedClass)).ToArray<string>(); 
            foreach(string negativeExampleRoot in negativeExampleRootPaths)
                _negativeExamplePaths.AddRange(System.IO.Directory.GetFiles(negativeExampleRoot));

            //Display subset of negative examples
            displayedExamples = 0;
            lastExampleXlocation = 10;

            negativeExampleImages.Controls.Clear();
            foreach(string examplePath in _negativeExamplePaths)
            {
                if(displayedExamples++ < maxDisplayedExamples)
                {
                    PictureBox exampleBox = new PictureBox();
                    Emgu.CV.Image<Gray, Byte> exampleImage = new Emgu.CV.Image<Bgr, int>(examplePath).Convert<Gray, Byte>();
                    lastExampleXlocation = lastExampleXlocation + exampleBox.Width + 5;
                    exampleBox.SetBounds(lastExampleXlocation, 10, exampleImage.Width, exampleImage.Height);
                    exampleBox.Image = exampleImage.ToBitmap();
                    negativeExampleImages.Controls.Add(exampleBox);
                }
            }
        }

        private void trainButton_Click(object sender, EventArgs e)
        {
            string classifiersDirectory = System.IO.Directory.GetCurrentDirectory()+"\\Classifiers";
            bool classifiersDirectoryExists = System.IO.Directory.Exists(classifiersDirectory);
            if(!classifiersDirectoryExists)
                System.IO.Directory.CreateDirectory(classifiersDirectory);

            string selectedClass = (string) classesComboBox.Items[classesComboBox.SelectedIndex];
            string classifierPath = classifiersDirectory + "\\" + selectedClass + "_classifier.xml";
            
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            string cascadeDirectory = System.IO.Directory.GetCurrentDirectory() + "\\cascade.xml";
            Emgu.CV.CascadeClassifier isCar = new Emgu.CV.CascadeClassifier(cascadeDirectory);
            string validationImagePath = System.IO.Directory.GetCurrentDirectory() + "\\Validate1.JPG";
            Emgu.CV.Image<Gray, Byte> validationImage = new Emgu.CV.Image<Bgr, int>(validationImagePath).Convert<Gray, Byte>();

            foreach (Rectangle r in isCar.DetectMultiScale(validationImage, 1.1, 3, new Size(30, 30), new Size(30, 30)))
            {
                infoBox.AppendText("Location: " + r.Location + Environment.NewLine);
                validationImage.Draw(r, new Gray(0), 2);
            }

            validationImagePictureBox.Image = validationImage.ToBitmap();
            validationImagePictureBox.Size = validationImage.Size;
            
        }



        

    }
}
