using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
    public partial class dnnForm : Form
    {
        private RBM _rbm;

        private Thread _trainingThread = null;
        private bool stopTraining = false;

        public dnnForm()
        {
            InitializeComponent();
        }

        private void selectTrainingImFolderButton_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                trainingPathTextbox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void trainRBMButton_Click(object sender, EventArgs e)
        {
            trainRBM();
        }

        private void trainRBM()
        {
            if(_rbm == null)
                CreateRBM();

            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
            _rbm.TrainMultithreaded(trainingData, Convert.ToInt32(trainingCyclesTextbox.Text));
        }
            

        private void exportWeightVisButton_Click_1(object sender, EventArgs e)
        {
            ExportWeightVisualizations();
        }

        private void ExportWeightVisualizations()
        {
            var dc = new RBMDataConverter();
            var exportPath = visualizationPathTextbox.Text;

            var maxWeight = 0.0;
            var minWeight = 0.0;
            var maxAfter = 255.0;
            var minAfter = 0.0;
            //foreach (var t1 in from t in _rbm.Weights from t1 in t where t1 > maxWeight select t1)
            //    maxWeight = t1;
            for (int i = 1; i < _rbm.Weights.Length; i++)
                for (int j = 1; j < _rbm.Weights[i].Length; j++)
                    if (_rbm.Weights[i][j] > maxWeight)
                        maxWeight = _rbm.Weights[i][j];


            //foreach (var t1 in from t in _rbm.Weights from t1 in t where t1 < minWeight select t1)
            //    minWeight = t1;
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


        private void showReconstructionsButton_Click(object sender, EventArgs e)
        {
            showReconstructions();
        }

        private void showReconstructions()
        {
            var exportPath = reconstructionPathTextbox.Text;
            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
            for (var i = 0; i < trainingData.Length; i++)
            {
                //var activations = _rbm.ComputeActivations(trainingData[i]);
                var activations = _rbm.ComputeActivationsExact(trainingData[i]);
                var reconstruction = _rbm.ReconstructExact(activations);
                dc.SaveDataToImage(reconstruction, exportPath + "\\" + i + ".bmp", 30, 30);
            } 
        }

        private void reconstructSingleButton_Click(object sender, EventArgs e)
        {
            var exportPath = singleImageTextbox.Text + "\\reconstruction.bmp";
            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(singleImageTextbox.Text);
            var activations = _rbm.ComputeActivationsExact(trainingData[0]);
            var reconstruction = _rbm.ReconstructExact(activations);
            dc.SaveDataToImage(reconstruction, exportPath, 30, 30);
        }

        private void startTrainingButton_Click(object sender, EventArgs e)
        {
            stopTraining = false;
            _trainingThread = new Thread(new ThreadStart(trainRBM));
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
            CreateRBM();
        }

        private void CreateRBM()
        {
            _rbm = new RBM(2701, Convert.ToInt16(hiddenUnitsTextbox.Text), Convert.ToDouble(learningRateTextbox.Text), 0.1);
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            ExportWeightVisualizations();
            showReconstructions();
        }

        private void exportWeightsButton_Click(object sender, EventArgs e)
        {
            exportWeights();
        }

        private void exportWeights()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(exportWeightsTextbox.Text, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _rbm);
            stream.Close();
        }

        private void importWeightsButton_Click(object sender, EventArgs e)
        {
            importWeights();
        }

        private void importWeights()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(importWeightsTextbox.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
            _rbm = (RBM) formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
