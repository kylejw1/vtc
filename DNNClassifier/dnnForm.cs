using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNNClassifier
{
    public partial class DnnForm : Form
    {
        private RBM _rbm;
        private NN _nn;

        private Thread _trainingThread;

        public DnnForm()
        {
            InitializeComponent();
            DragEnter += Form_DragEnter;
            DragDrop += singleImageTextbox_DragDrop;
        }

        /// <summary>
        /// Check if the Dataformat of the data can be accepted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Form_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void trainRBMButton_Click(object sender, EventArgs e)
        {
            TrainRbm();
        }

        private void TrainRbm()
        {
            if(_rbm == null)
                throw new MemberAccessException("Cannot train, RBM does not exist. Create RBM first.");

            var trainingData = GetTrainingSetFromTextbox();
            _rbm.TrainMultithreaded(trainingData);
        }
            

        private void exportWeightVisButton_Click_1(object sender, EventArgs e)
        {
            ExportWeightVisualizations();
        }

        private void ExportWeightVisualizations()
        {
            _rbm.ExportWeightVisualizations(Directory.GetCurrentDirectory() + visualizationPathTextbox.Text);
        }


        private void showReconstructionsButton_Click(object sender, EventArgs e)
        {
            ShowReconstructions();
        }

        private void ShowReconstructions()
        {
            var exportPath = Directory.GetCurrentDirectory() + reconstructionPathTextbox.Text;
            var dc = new RBMDataConverter();
            var trainingData = GetTrainingSetFromTextbox();
            for (var i = 0; i < trainingData.Length; i++)
            {
                var activations = _rbm.ComputeActivationsExact(trainingData[i]);
                var reconstruction = _rbm.ReconstructExact(activations);
                dc.SaveDataToImage(reconstruction, exportPath + "\\" + i + ".bmp", 30, 30);
            }        
        }

        private double[][] GetTrainingSetFromTextbox()
        {
            var dc = new RBMDataConverter();
            var pathString = Directory.GetCurrentDirectory() + trainingPathTextbox.Text;
            var trainingData = dc.TrainingSetFromPath(pathString);
            return trainingData;
        }

        private void reconstructSingleButton_Click(object sender, EventArgs e)
        {
            var exportPath = Path.GetDirectoryName(singleImageTextbox.Text) + "\\" + Path.GetFileNameWithoutExtension(singleImageTextbox.Text) + "reconstruction.bmp";
            var activations = _rbm.ExportSingleReconstruction(singleImageTextbox.Text, exportPath);

            var im = Image.FromFile(singleImageTextbox.Text);
            inputPictureBox.Image = im;
            var imRec = Image.FromFile(exportPath);
            reconstructionPictureBox.Image = imRec;

            if (_nn != null)
                classifierOutputTextbox.Text = _nn.ClassifyInput(activations);
        }

        private void startTrainingButton_Click(object sender, EventArgs e)
        {
            _trainingThread = new Thread(TrainRbm);
            _trainingThread.Start();
            renderTimer.Enabled = true;
        }

        private void stopTrainingButton_Click(object sender, EventArgs e)
        {
            renderTimer.Enabled = false;
            _rbm.StopTrainMultithreaded();
        }

        private void createRBMButton_Click(object sender, EventArgs e)
        {
            _rbm = new RBM(2701, Convert.ToInt16(hiddenUnitsTextbox.Text), Convert.ToDouble(learningRateTextbox.Text), 0.1);
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            if (!exportImagesCheckbox.Checked) return;
            ExportWeightVisualizations();
            ShowReconstructions();
        }

        private void exportWeightsButton_Click(object sender, EventArgs e)
        {
            ExportRbmWeights();
        }

        private void ExportRbmWeights()
        {
            _rbm.ExportWeights(Directory.GetCurrentDirectory() + exportRBMWeightsTextbox.Text);
        }

        private void ExportNNWeights()
        {
            _nn.ExportWeights(Directory.GetCurrentDirectory() + NNWeightsPathTextbox.Text);
        }

        private void importWeightsButton_Click(object sender, EventArgs e)
        {
            ImportRBMWeights();
        }

        private void ImportRBMWeights()
        {
            _rbm = RBM.ImportWeights(Directory.GetCurrentDirectory() + exportRBMWeightsTextbox.Text);
            hiddenUnitsTextbox.Text = _rbm.Weights.Length.ToString();
        }

        private void ImportNNWeights()
        {
            _nn = NN.ImportWeights(Directory.GetCurrentDirectory() + NNWeightsPathTextbox.Text);
        }

        private void createNNButton_Click(object sender, EventArgs e)
        {
            CreateNN();
        }

        private void CreateNN()
        {
            var classes = ClassNames();
            _nn = new NN(Convert.ToInt32(hiddenUnitsTextbox.Text), classes.Length,
                Convert.ToDouble(learningRateTextbox.Text), classes); 
        }

        private string[] ClassNames()
        {
            var labelledImagesPath =Directory.GetCurrentDirectory() + labelledImagePathTextbox.Text;
            var paths = Directory.GetDirectories(labelledImagesPath);
            var classes = paths.Select(Path.GetFileName).ToArray();
            return classes;
        }

        private int CountLabelledExamples()
        {
            var labelledImagesPath =Directory.GetCurrentDirectory() + labelledImagePathTextbox.Text;
            var paths = Directory.GetDirectories(labelledImagesPath);
            return paths.Sum(t => Directory.GetFiles(t).Count());
        }

        private void trainNNButton_Click(object sender, EventArgs e)
        {
            CalculateActivations();
            _nn.Train(_nn.Inputs, _nn.Targets, Convert.ToInt32(trainingCyclesTextbox.Text));
            var error = _nn.AverageError(_nn.Inputs, _nn.Targets);
            classifierErrorTextbox.Text = error.ToString(CultureInfo.InvariantCulture);
        }

        private void exportNNButton_Click(object sender, EventArgs e)
        {
            ExportNNWeights();
        }

        private void importNNButton_Click(object sender, EventArgs e)
        {
            ImportNNWeights();
        }

        private void singleImageTextbox_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                singleImageTextbox.Text = file;
                reconstructSingleButton_Click(sender, e);    
            }
        }

        private void loadAllButton_Click(object sender, EventArgs e)
        {
            ImportRBMWeights();
            importNNButton_Click(sender, e);
        }

        private void calculateActivationsButton_Click(object sender, EventArgs e)
        {
            CalculateActivations();
        }

        private void CalculateActivations()
        {
            //Calculate activations
            var labelledImagesPath = Directory.GetCurrentDirectory() + labelledImagePathTextbox.Text;
            var paths = Directory.GetDirectories(labelledImagesPath);
            var classes = ClassNames();
            var classCount = classes.Length;

            //Get total number of training examples
            var numTrainingSamples = CountLabelledExamples();

            var inputs = new double[numTrainingSamples][];
            var targets = new double[numTrainingSamples][];

            var sampleIndex = 0;

            for (var i = 0; i < classCount; i++)
            {
                var dc = new RBMDataConverter();
                var trainingData = dc.TrainingSetFromPath(paths[i]);
                var thisSetCount = trainingData.Length;
                for (var j = 0; j < thisSetCount; j++)
                {
                    inputs[sampleIndex] = _rbm.ComputeActivationsExact(trainingData[j]);
                    targets[sampleIndex] = new double[classCount];
                    targets[sampleIndex][i] = 1;
                    sampleIndex++;
                }
            }

            if (_nn == null)
                throw new MemberAccessException("Cannot use RBM activations if NN does not exist. First create NN.");

            _nn.Inputs = inputs;
            _nn.Targets = targets;
        }

    }
}
