using System;
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

            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
            _rbm.TrainMultithreaded(trainingData);
        }
            

        private void exportWeightVisButton_Click_1(object sender, EventArgs e)
        {
            ExportWeightVisualizations();
        }

        private void ExportWeightVisualizations()
        {
            if (exportImagesCheckbox.Checked)
            {
                var dc = new RBMDataConverter();
                var exportPath = visualizationPathTextbox.Text;

                var maxWeight = 0.0;
                var minWeight = 0.0;
                var maxAfter = 255.0;
                var minAfter = 0.0;

                for (int i = 1; i < _rbm.Weights.Length; i++)
                    for (int j = 1; j < _rbm.Weights[i].Length; j++)
                        if (_rbm.Weights[i][j] > maxWeight)
                            maxWeight = _rbm.Weights[i][j];

                for (int i = 1; i < _rbm.Weights.Length; i++)
                    for (int j = 1; j < _rbm.Weights[i].Length; j++)
                        if (_rbm.Weights[i][j] < minWeight)
                            minWeight = _rbm.Weights[i][j];

                var transformedWeights = new double[_rbm.Weights.Length][];
                for (var i = 0; i < _rbm.Weights.Length; i++)
                {
                    var newWeightsArray = new double[_rbm.Weights[0].Length];
                    for (int j = 0; j < _rbm.Weights[i].Length; j++)
                        newWeightsArray[j] = (_rbm.Weights[i][j] - minWeight) * (maxAfter - minAfter) / (maxWeight - minWeight);

                    transformedWeights[i] = newWeightsArray;
                }

                for (var i = 0; i < _rbm.Weights.Length; i++)
                {
                    dc.SaveRawDataToImage(transformedWeights[i], exportPath + "\\" + i + ".bmp", 30, 30);
                }
            }
        }


        private void showReconstructionsButton_Click(object sender, EventArgs e)
        {
            ShowReconstructions();
        }

        private void ShowReconstructions()
        {
            if (exportImagesCheckbox.Checked)
            {
                var exportPath = reconstructionPathTextbox.Text;
                var dc = new RBMDataConverter();
                var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
                for (var i = 0; i < trainingData.Length; i++)
                {
                    var activations = _rbm.ComputeActivationsExact(trainingData[i]);
                    var reconstruction = _rbm.ReconstructExact(activations);
                    dc.SaveDataToImage(reconstruction, exportPath + "\\" + i + ".bmp", 30, 30);
                }     
            }
        }

        private void reconstructSingleButton_Click(object sender, EventArgs e)
        {
            var exportPath = Path.GetDirectoryName(singleImageTextbox.Text) + "\\" + Path.GetFileNameWithoutExtension(singleImageTextbox.Text) + "reconstruction.bmp";

            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromSingleImage(singleImageTextbox.Text);
            var activations = _rbm.ComputeActivationsExact(trainingData[0]);
            var reconstruction = _rbm.ReconstructExact(activations);
            dc.SaveDataToImage(reconstruction, exportPath, 30, 30);

            if (_nn != null)
            {
                double[] output = _nn.Evaluate(activations);
                string[] classes = ClassNames();
                double maxClassifierOutput = output.Max();
                int maxIndex = output.ToList().IndexOf(maxClassifierOutput);
                classifierOutputTextbox.Text = classes[maxIndex];    
            }

            var im = Image.FromFile(singleImageTextbox.Text);
            inputPictureBox.Image = im;
            var imRec = Image.FromFile(exportPath);
            reconstructionPictureBox.Image = imRec;

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
            ExportWeightVisualizations();
            ShowReconstructions();
        }

        private void exportWeightsButton_Click(object sender, EventArgs e)
        {
            ExportRbmWeights();
        }

        private void ExportRbmWeights()
        {
            _rbm.ExportWeights(exportRBMWeightsTextbox.Text);
        }

        private void ExportNNWeights()
        {
            _nn.ExportWeights(NNWeightsPathTextbox.Text);
        }

        private void importWeightsButton_Click(object sender, EventArgs e)
        {
            ImportRBMWeights();
        }

        private void ImportRBMWeights()
        {
            _rbm = RBM.ImportWeights(exportRBMWeightsTextbox.Text);
        }

        private void ImportNNWeights()
        {
            _nn = NN.ImportWeights(NNWeightsPathTextbox.Text);
        }

        private void createNNButton_Click(object sender, EventArgs e)
        {
            CreateNN();
        }

        private void CreateNN()
        {
            var classes = ClassNames();
            _nn = new NN(Convert.ToInt32(hiddenUnitsTextbox.Text), classes.Length,
                Convert.ToDouble(learningRateTextbox.Text)); 
        }

        private void exportLabelledRBMActivationsButton_Click(object sender, EventArgs e)
        {
            //Calculate activations
            var labelledImagesPath = labelledImagePathTextbox.Text;
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

            //Export

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(hiddenActivationsPathTextbox.Text + "activations.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, inputs);
            stream.Close();

            Stream stream2 = new FileStream(hiddenActivationsPathTextbox.Text + "targets.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream2, targets);
            stream2.Close();

            if (_nn == null)
                CreateNN();

            _nn.Inputs = inputs;
            _nn.Targets = targets;
        }

        private void importRBMActivationsButton_Click(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(hiddenActivationsPathTextbox.Text + "activations.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            var inputs = (double[][]) formatter.Deserialize(stream);
            stream.Close();

            Stream stream2 = new FileStream(hiddenActivationsPathTextbox.Text + "targets.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            var targets = (double[][])formatter.Deserialize(stream2);
            stream2.Close();

            if (_nn == null)
                CreateNN();

            _nn.Inputs = inputs;
            _nn.Targets = targets;
        }

        private string[] ClassNames()
        {
            var labelledImagesPath = labelledImagePathTextbox.Text;
            var paths = Directory.GetDirectories(labelledImagesPath);
            var classes = paths.Select(Path.GetFileName).ToArray();
            return classes;
        }

        private int CountLabelledExamples()
        {
            var labelledImagesPath = labelledImagePathTextbox.Text;
            var paths = Directory.GetDirectories(labelledImagesPath);
            return paths.Sum(t => Directory.GetFiles(t).Count());
        }

        private void trainNNButton_Click(object sender, EventArgs e)
        {
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

    }
}
