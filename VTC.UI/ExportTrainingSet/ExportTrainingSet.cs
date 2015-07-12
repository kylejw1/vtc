using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VTC.Settings;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.IO;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using VTC.Kernel;


namespace VTC.ExportTrainingSet
{
    public partial class ExportTrainingSet : Form
    {
        private readonly AppSettings _settings;
        private Image<Bgr, float> _frame;
        private Image<Gray, byte> _movementMask;
        private PictureBox _picture = new PictureBox();
        private PictureBox _subimage = new PictureBox();
        private List<RadioButton> classRadioButtons = new List<RadioButton>();
        private List<Vehicle> _vehicles;
        private SortedList<CvBlob, int> _blobsWithArea;

        int _unpaddedX;
        int _unpaddedY;

        public ExportTrainingSet(AppSettings settings, Image<Bgr, float> bgImage, List<Vehicle> currentVehicles, Image<Gray, byte> movementMask)
        {
            _settings = settings;
            _vehicles = new List<Vehicle>();
            foreach(Vehicle v in currentVehicles)
                _vehicles.Add(v);

            _unpaddedX = bgImage.Width;
            _unpaddedY = bgImage.Height;

            int paddedWidth = bgImage.Width + _settings.ClassifierSubframeWidth;
            int paddedHeight = bgImage.Height + _settings.ClassifierSubframeHeight;
            _frame = new Image<Bgr, float>(paddedWidth, paddedHeight);
            _movementMask = new Image<Gray, byte>(paddedWidth, paddedHeight);
            CopySubimage(bgImage, _frame);
            CopySubimage(movementMask, _movementMask);
            

            _picture.Width = _frame.Width;
            _picture.Height = _frame.Height;
            _picture.Image = _frame.ToBitmap();

            InitializeComponent();

            subimagePanel.Width = _settings.ClassifierSubframeWidth;
            subimagePanel.Height = _settings.ClassifierSubframeHeight;

            imagePanel.MaximumSize = _picture.Size;
            imagePanel.MinimumSize = _picture.Size;
            imagePanel.Size = _picture.Size;
            System.Diagnostics.Debug.WriteLine("Size is " + _picture.Size);
            imagePanel.Controls.Add(_picture);
            CreateClassRadioButtons();

            _picture.MouseClick += imagePanel_MouseClick;
        }

        private void CreateClassRadioButtons()
        {
            int lastRadioButtonPosition = 0;

            foreach(string classString in _settings.Classes)
        {
            RadioButton classRadioButton = new RadioButton();
            lastRadioButtonPosition = lastRadioButtonPosition + classRadioButton.Height + 5;          
            objectClassGroupBox.Controls.Add(classRadioButton);
            classRadioButton.AutoSize = true;
            classRadioButton.Location = new Point(6,lastRadioButtonPosition);
            classRadioButton.Name = classString+"RadioButton";
            classRadioButton.Size = TextRenderer.MeasureText(classString, classRadioButton.Font);
            classRadioButton.TabIndex = 0;
            classRadioButton.TabStop = true;
            classRadioButton.Text = classString;
            classRadioButton.UseVisualStyleBackColor = true;
            classRadioButton.MouseClick += radioButton_CheckedChanged;
            classRadioButtons.Add(classRadioButton);
                
        }
            
        }

        private void CopySubimage(Image<Bgr, float> source, Image<Bgr, float> destination)
        {
            int x_offset = (destination.Width - source.Width) / 2 - 1;
            int y_offset = (destination.Height - source.Height) / 2 - 1;
            for(int i=0; i<source.Width;i++)
                for(int j=0; j<source.Height; j++)
                {
                    destination.Data[j + y_offset, i + x_offset, 0] = source.Data[j, i, 0];
                    destination.Data[j + y_offset, i + x_offset, 1] = source.Data[j, i, 1];
                    destination.Data[j + y_offset, i + x_offset, 2] = source.Data[j, i, 2];
                }
        }

        private void CopySubimage(Image<Gray, byte> source, Image<Gray, byte> destination)
        {
            int x_offset = (destination.Width - source.Width) / 2 - 1;
            int y_offset = (destination.Height - source.Height) / 2 - 1;
            for (int i = 0; i < source.Width; i++)
                for (int j = 0; j < source.Height; j++)
                {
                    destination.Data[j + y_offset, i + x_offset,0] = source.Data[j, i,0];
                }
        }

        private void imagePanel_MouseClick(object sender, MouseEventArgs e)
        {
            int minX = _settings.ClassifierSubframeWidth/2;
            int maxX = _picture.Width - _settings.ClassifierSubframeWidth / 2 - 1;
            int minY = _settings.ClassifierSubframeHeight / 2;
            int maxY = _picture.Height - _settings.ClassifierSubframeHeight / 2 - 1;
            if (e.X >= minX && e.X <= maxX && e.Y >= minY && e.Y <= maxY)
            {
                int subimageWidth = _settings.ClassifierSubframeWidth;
                int subimageHeight = _settings.ClassifierSubframeHeight;
                int x = e.X - subimageWidth / 2;
                int y = e.Y - subimageHeight / 2;
                Image<Bgr, float> subimage = _frame.GetSubRect(new Rectangle(x, y, subimageWidth, subimageHeight));

                _subimage.Size = subimage.Size;
                _subimage.Image = subimage.ToBitmap();

                subimagePanel.Width = _settings.ClassifierSubframeWidth;
                subimagePanel.Height = _settings.ClassifierSubframeHeight;

                subimagePanel.Controls.Add(_subimage);
                subimagePanel.Refresh();
            }
            else
                infoBox.AppendText("Error: cannot select padded outline. Click inside the training image. " + Environment.NewLine);

            foreach(RadioButton radioButton in classRadioButtons)
                radioButton.Checked = false;
            
            
        }

        private Image<Bgr, float> extractSubImage(int x, int y)
        {
            int subimageWidth = _settings.ClassifierSubframeWidth;
            int subimageHeight = _settings.ClassifierSubframeHeight;
            //Image<Bgr, float> subimage = _frame.GetSubRect(new Rectangle(x+subimageWidth/2, y+subimageHeight/2, subimageWidth, subimageHeight));
            Image<Bgr, float> subimage = _frame.GetSubRect(new Rectangle(x, y, subimageWidth, subimageHeight));
            return subimage;
        }

        private Image<Bgr, float> extractScaledSubImage(CvBlob blob)
        {
            int subimageWidth = _settings.ClassifierSubframeWidth;
            int subimageHeight = _settings.ClassifierSubframeHeight;

            Image<Bgr, float> subimageUnscaled = _frame.GetSubRect(blob.BoundingBox);
            Image<Bgr, float> subimage = subimageUnscaled.Resize(subimageWidth, subimageHeight, INTER.CV_INTER_AREA);

            return subimage;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            //Get class string
            RadioButton rb = (RadioButton)sender;
            string classString = rb.Text;
            infoBox.AppendText("Class: " + classString + " saved." + Environment.NewLine);
            string examplePath = constructExamplePath(classString);
            _subimage.Image.Save(examplePath);
        }

        private void saveExampleImage(Image<Bgr,float> image, string classString)
        {
            string examplePath = constructExamplePath(classString);
            if(exportMonoTrainingSamples.Checked)
                image.Convert<Gray, Byte>().Save(examplePath);
            else
                image.Save(examplePath);
        }

        private static string constructExamplePath(string classString)
        {
            string examplesDirectory = createExamplesDirectoryIfNotExists();
            string classDirectory = createClassDirectoryIfNotExists(classString, examplesDirectory);
            List<Int16> filenamesToNumbers = Directory.GetFiles(classDirectory).ToList<String>().Select(s => Convert.ToInt16(System.IO.Path.GetFileNameWithoutExtension(s))).ToList();
            filenamesToNumbers.Add(0);
            filenamesToNumbers.Sort();
            int newExampleNum = filenamesToNumbers.Last() +1;
            string examplePath = classDirectory + "\\" + newExampleNum + ".bmp";
            return examplePath;
        }

        private static string createClassDirectoryIfNotExists(string classString, string examplesDirectory)
        {
            string classDirectory = examplesDirectory + "\\" + classString;
            Boolean classDirectoryExists = Directory.Exists(classDirectory);
            if (!classDirectoryExists)
               Directory.CreateDirectory(classDirectory);

            return classDirectory;
        }

        private static string createExamplesDirectoryIfNotExists()
        {
            string examplesDirectory = Directory.GetCurrentDirectory() + "\\examples";
            Boolean examplesDirectoryExists = Directory.Exists(examplesDirectory);
            if (!examplesDirectoryExists)
                Directory.CreateDirectory(examplesDirectory);
            return examplesDirectory;
        }

        private void autoExportButton_Click(object sender, EventArgs e)
        {
            ExportPositives();   
            ExportNegatives();
        }

        private void ExportNegatives()
        {
            int sampleStepSize = 5;
            int samplePadding = 15;
            for (int i = 0; i < _unpaddedX; i += sampleStepSize)
                for (int j = 0; j < _unpaddedY; j += sampleStepSize)
                {

                    bool vehicleIsNearby = false;
                    foreach (Vehicle v in _vehicles)
                    {
                        int vehicleX = (int)v.StateHistory.Last().X;
                        int vehicleY = (int)v.StateHistory.Last().Y;
                        if (i < vehicleX + samplePadding && i > vehicleX - samplePadding && j < vehicleY + samplePadding && j > vehicleY - samplePadding)
                        {
                            vehicleIsNearby = true;
                            break;
                        }
                    }

                    if (!vehicleIsNearby)
                    {
                        Image<Bgr, float> subimage = extractSubImage(i, j);
                        saveExampleImage(subimage, "Other"); //TODO: Maybe this shouldn't be hardcoded
                    }

                }
        }

        private void autoExportPositives_Click(object sender, EventArgs e)
        {
            ExportPositives();
        }

        private void ExportPositives()
        {
            if (scaleBlobsCheckbox.Checked)
            {
                var bf = new BlobFinder();
                var blobsWithSizes = bf.FindBlobs(_movementMask, _settings.MinObjectSize);
                foreach (var blobWithArea in blobsWithSizes)
                {
                    Image<Bgr, float> subimage = extractScaledSubImage(blobWithArea.Key);
                    saveExampleImage(subimage, "Car"); //TODO: Maybe this shouldn't be hardcoded
                }    
            }
            else
            {
                foreach (Vehicle v in _vehicles)
                {
                    int vehicleX = (int)v.StateHistory.Last<StateEstimate>().X;
                    int vehicleY = (int)v.StateHistory.Last<StateEstimate>().Y;
                    if (vehicleX > _settings.ClassifierSubframeWidth && vehicleX < _frame.Width - _settings.ClassifierSubframeWidth && vehicleY > _settings.ClassifierSubframeHeight && vehicleY < _frame.Height - _settings.ClassifierSubframeHeight)
                    {
                        Image<Bgr, float> subimage = extractSubImage(vehicleX, vehicleY);
                        saveExampleImage(subimage, "Car"); //TODO: Maybe this shouldn't be hardcoded
                    }
                }    
            }
        }

    }
}
