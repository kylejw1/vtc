using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNNClassifier
{
    public partial class dnnForm : Form
    {
        private RBM _rbm;

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
            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
            _rbm = new RBM(2700, Convert.ToInt16(hiddenUnitsTextbox.Text),Convert.ToDouble(learningRateTextbox.Text), 0.1);
            _rbm.Train(trainingData, Convert.ToInt32(trainingCyclesTextbox.Text));
        }

        private void exportWeightVisButton_Click_1(object sender, EventArgs e)
        {
            var dc = new RBMDataConverter();
            var exportPath = visualizationPathTextbox.Text;
            for (var i = 0; i < _rbm.Weights.Length; i++)
            {
                dc.SaveDataToImage(_rbm.Weights[i], exportPath + "\\" + i + ".bmp", 30, 30);
            }
        }

        private void showReconstructionsButton_Click(object sender, EventArgs e)
        {
            var exportPath = reconstructionPathTextbox.Text;
            var dc = new RBMDataConverter();
            var trainingData = dc.TrainingSetFromPath(trainingPathTextbox.Text);
            for (var i = 0; i < trainingData.Length; i++)
            {
                var activations = _rbm.ComputeActivations(trainingData[i]);
                var reconstruction = _rbm.Reconstruct(activations);
                dc.SaveDataToImage(reconstruction, exportPath + "\\" + i + ".bmp", 30, 30);
            }
        }
    }
}
