namespace DNNClassifier
{
    partial class dnnForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.trainingPathTextbox = new System.Windows.Forms.TextBox();
            this.trainRBMButton = new System.Windows.Forms.Button();
            this.exportWeightVisButton = new System.Windows.Forms.Button();
            this.showReconstructionsButton = new System.Windows.Forms.Button();
            this.reconstructionPathTextbox = new System.Windows.Forms.TextBox();
            this.visualizationPathTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.hiddenUnitsTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.trainingCyclesTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.learningRateLabel = new System.Windows.Forms.Label();
            this.learningRateTextbox = new System.Windows.Forms.TextBox();
            this.singleImageTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.reconstructSingleButton = new System.Windows.Forms.Button();
            this.startTrainingButton = new System.Windows.Forms.Button();
            this.stopTrainingButton = new System.Windows.Forms.Button();
            this.createRBMButton = new System.Windows.Forms.Button();
            this.renderTimer = new System.Windows.Forms.Timer(this.components);
            this.exportWeightsButton = new System.Windows.Forms.Button();
            this.importWeightsButton = new System.Windows.Forms.Button();
            this.exportWeightsTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.hiddenActivationsPathTextbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.exportRBMActivationsButton = new System.Windows.Forms.Button();
            this.importRBMActivationsButton = new System.Windows.Forms.Button();
            this.trainNNButton = new System.Windows.Forms.Button();
            this.createNNButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.labelledImagePathTextbox = new System.Windows.Forms.TextBox();
            this.classifierErrorTextbox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.classifierOutputTextbox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.exportNNButton = new System.Windows.Forms.Button();
            this.importNNButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trainingPathTextbox
            // 
            this.trainingPathTextbox.Location = new System.Drawing.Point(12, 12);
            this.trainingPathTextbox.Name = "trainingPathTextbox";
            this.trainingPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.trainingPathTextbox.TabIndex = 1;
            this.trainingPathTextbox.Text = "C:\\Emgu\\emgucv-windows-universal 3.0.0.2157\\Emgu.CV.Example\\vtc_eugene3\\bin\\examp" +
    "les\\Car";
            // 
            // trainRBMButton
            // 
            this.trainRBMButton.Location = new System.Drawing.Point(719, 381);
            this.trainRBMButton.Name = "trainRBMButton";
            this.trainRBMButton.Size = new System.Drawing.Size(146, 25);
            this.trainRBMButton.TabIndex = 2;
            this.trainRBMButton.Text = "Train RBM";
            this.trainRBMButton.UseVisualStyleBackColor = true;
            this.trainRBMButton.Click += new System.EventHandler(this.trainRBMButton_Click);
            // 
            // exportWeightVisButton
            // 
            this.exportWeightVisButton.Location = new System.Drawing.Point(719, 105);
            this.exportWeightVisButton.Name = "exportWeightVisButton";
            this.exportWeightVisButton.Size = new System.Drawing.Size(146, 25);
            this.exportWeightVisButton.TabIndex = 3;
            this.exportWeightVisButton.Text = "Export Weight Vis";
            this.exportWeightVisButton.UseVisualStyleBackColor = true;
            this.exportWeightVisButton.Click += new System.EventHandler(this.exportWeightVisButton_Click_1);
            // 
            // showReconstructionsButton
            // 
            this.showReconstructionsButton.Location = new System.Drawing.Point(719, 321);
            this.showReconstructionsButton.Name = "showReconstructionsButton";
            this.showReconstructionsButton.Size = new System.Drawing.Size(146, 25);
            this.showReconstructionsButton.TabIndex = 4;
            this.showReconstructionsButton.Text = "Show Reconstructions";
            this.showReconstructionsButton.UseVisualStyleBackColor = true;
            this.showReconstructionsButton.Click += new System.EventHandler(this.showReconstructionsButton_Click);
            // 
            // reconstructionPathTextbox
            // 
            this.reconstructionPathTextbox.Location = new System.Drawing.Point(12, 38);
            this.reconstructionPathTextbox.Name = "reconstructionPathTextbox";
            this.reconstructionPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.reconstructionPathTextbox.TabIndex = 6;
            this.reconstructionPathTextbox.Text = "C:\\Emgu\\emgucv-windows-universal 3.0.0.2157\\Emgu.CV.Example\\vtc_eugene3\\bin\\examp" +
    "les\\reconstructions";
            // 
            // visualizationPathTextbox
            // 
            this.visualizationPathTextbox.Location = new System.Drawing.Point(12, 64);
            this.visualizationPathTextbox.Name = "visualizationPathTextbox";
            this.visualizationPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.visualizationPathTextbox.TabIndex = 7;
            this.visualizationPathTextbox.Text = "C:\\Emgu\\emgucv-windows-universal 3.0.0.2157\\Emgu.CV.Example\\vtc_eugene3\\bin\\examp" +
    "les\\visualizations";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(578, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Training images path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(578, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Reconstructions path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(578, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Visualizations path";
            // 
            // hiddenUnitsTextbox
            // 
            this.hiddenUnitsTextbox.Location = new System.Drawing.Point(471, 201);
            this.hiddenUnitsTextbox.Name = "hiddenUnitsTextbox";
            this.hiddenUnitsTextbox.Size = new System.Drawing.Size(100, 20);
            this.hiddenUnitsTextbox.TabIndex = 11;
            this.hiddenUnitsTextbox.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(577, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "RBM hidden units";
            // 
            // trainingCyclesTextbox
            // 
            this.trainingCyclesTextbox.Location = new System.Drawing.Point(471, 226);
            this.trainingCyclesTextbox.Name = "trainingCyclesTextbox";
            this.trainingCyclesTextbox.Size = new System.Drawing.Size(100, 20);
            this.trainingCyclesTextbox.TabIndex = 13;
            this.trainingCyclesTextbox.Text = "5000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(578, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Training cycles";
            // 
            // learningRateLabel
            // 
            this.learningRateLabel.AutoSize = true;
            this.learningRateLabel.Location = new System.Drawing.Point(578, 255);
            this.learningRateLabel.Name = "learningRateLabel";
            this.learningRateLabel.Size = new System.Drawing.Size(69, 13);
            this.learningRateLabel.TabIndex = 16;
            this.learningRateLabel.Text = "Learning rate";
            // 
            // learningRateTextbox
            // 
            this.learningRateTextbox.Location = new System.Drawing.Point(471, 252);
            this.learningRateTextbox.Name = "learningRateTextbox";
            this.learningRateTextbox.Size = new System.Drawing.Size(100, 20);
            this.learningRateTextbox.TabIndex = 15;
            this.learningRateTextbox.Text = "0.001";
            // 
            // singleImageTextbox
            // 
            this.singleImageTextbox.Location = new System.Drawing.Point(12, 141);
            this.singleImageTextbox.Name = "singleImageTextbox";
            this.singleImageTextbox.Size = new System.Drawing.Size(559, 20);
            this.singleImageTextbox.TabIndex = 17;
            this.singleImageTextbox.Text = "C:\\Emgu\\emgucv-windows-universal 3.0.0\\Emgu.CV.Example\\vtc_eugene3\\bin\\examples\\S" +
    "ingle Image\\";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(577, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Single image path";
            // 
            // reconstructSingleButton
            // 
            this.reconstructSingleButton.Location = new System.Drawing.Point(719, 352);
            this.reconstructSingleButton.Name = "reconstructSingleButton";
            this.reconstructSingleButton.Size = new System.Drawing.Size(146, 23);
            this.reconstructSingleButton.TabIndex = 20;
            this.reconstructSingleButton.Text = "Reconstruct single";
            this.reconstructSingleButton.UseVisualStyleBackColor = true;
            this.reconstructSingleButton.Click += new System.EventHandler(this.reconstructSingleButton_Click);
            // 
            // startTrainingButton
            // 
            this.startTrainingButton.Location = new System.Drawing.Point(719, 43);
            this.startTrainingButton.Name = "startTrainingButton";
            this.startTrainingButton.Size = new System.Drawing.Size(146, 25);
            this.startTrainingButton.TabIndex = 21;
            this.startTrainingButton.Text = "Start training";
            this.startTrainingButton.UseVisualStyleBackColor = true;
            this.startTrainingButton.Click += new System.EventHandler(this.startTrainingButton_Click);
            // 
            // stopTrainingButton
            // 
            this.stopTrainingButton.Location = new System.Drawing.Point(719, 74);
            this.stopTrainingButton.Name = "stopTrainingButton";
            this.stopTrainingButton.Size = new System.Drawing.Size(146, 25);
            this.stopTrainingButton.TabIndex = 22;
            this.stopTrainingButton.Text = "Stop training";
            this.stopTrainingButton.UseVisualStyleBackColor = true;
            this.stopTrainingButton.Click += new System.EventHandler(this.stopTrainingButton_Click);
            // 
            // createRBMButton
            // 
            this.createRBMButton.Location = new System.Drawing.Point(719, 12);
            this.createRBMButton.Name = "createRBMButton";
            this.createRBMButton.Size = new System.Drawing.Size(146, 25);
            this.createRBMButton.TabIndex = 23;
            this.createRBMButton.Text = "Create RBM";
            this.createRBMButton.UseVisualStyleBackColor = true;
            this.createRBMButton.Click += new System.EventHandler(this.createRBMButton_Click);
            // 
            // renderTimer
            // 
            this.renderTimer.Interval = 5000;
            this.renderTimer.Tick += new System.EventHandler(this.renderTimer_Tick);
            // 
            // exportWeightsButton
            // 
            this.exportWeightsButton.Location = new System.Drawing.Point(719, 136);
            this.exportWeightsButton.Name = "exportWeightsButton";
            this.exportWeightsButton.Size = new System.Drawing.Size(146, 25);
            this.exportWeightsButton.TabIndex = 24;
            this.exportWeightsButton.Text = "Export RBM weights";
            this.exportWeightsButton.UseVisualStyleBackColor = true;
            this.exportWeightsButton.Click += new System.EventHandler(this.exportWeightsButton_Click);
            // 
            // importWeightsButton
            // 
            this.importWeightsButton.Location = new System.Drawing.Point(719, 167);
            this.importWeightsButton.Name = "importWeightsButton";
            this.importWeightsButton.Size = new System.Drawing.Size(146, 25);
            this.importWeightsButton.TabIndex = 25;
            this.importWeightsButton.Text = "Import RBM weights";
            this.importWeightsButton.UseVisualStyleBackColor = true;
            this.importWeightsButton.Click += new System.EventHandler(this.importWeightsButton_Click);
            // 
            // exportWeightsTextbox
            // 
            this.exportWeightsTextbox.Location = new System.Drawing.Point(12, 90);
            this.exportWeightsTextbox.Name = "exportWeightsTextbox";
            this.exportWeightsTextbox.Size = new System.Drawing.Size(559, 20);
            this.exportWeightsTextbox.TabIndex = 26;
            this.exportWeightsTextbox.Text = "C:\\Users\\TOSHIBA\\Desktop\\Weights\\rbm_weights.bin";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(577, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Weights  path";
            // 
            // hiddenActivationsPathTextbox
            // 
            this.hiddenActivationsPathTextbox.Location = new System.Drawing.Point(12, 116);
            this.hiddenActivationsPathTextbox.Name = "hiddenActivationsPathTextbox";
            this.hiddenActivationsPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.hiddenActivationsPathTextbox.TabIndex = 31;
            this.hiddenActivationsPathTextbox.Text = "C:\\Users\\TOSHIBA\\Desktop\\Activations\\";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(578, 119);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Hidden Activations Path";
            // 
            // exportRBMActivationsButton
            // 
            this.exportRBMActivationsButton.Location = new System.Drawing.Point(719, 197);
            this.exportRBMActivationsButton.Name = "exportRBMActivationsButton";
            this.exportRBMActivationsButton.Size = new System.Drawing.Size(146, 25);
            this.exportRBMActivationsButton.TabIndex = 33;
            this.exportRBMActivationsButton.Text = "Export RBM activations";
            this.exportRBMActivationsButton.UseVisualStyleBackColor = true;
            this.exportRBMActivationsButton.Click += new System.EventHandler(this.exportLabelledRBMActivationsButton_Click);
            // 
            // importRBMActivationsButton
            // 
            this.importRBMActivationsButton.Location = new System.Drawing.Point(719, 228);
            this.importRBMActivationsButton.Name = "importRBMActivationsButton";
            this.importRBMActivationsButton.Size = new System.Drawing.Size(146, 25);
            this.importRBMActivationsButton.TabIndex = 34;
            this.importRBMActivationsButton.Text = "Import RBM activations";
            this.importRBMActivationsButton.UseVisualStyleBackColor = true;
            this.importRBMActivationsButton.Click += new System.EventHandler(this.importRBMActivationsButton_Click);
            // 
            // trainNNButton
            // 
            this.trainNNButton.Location = new System.Drawing.Point(719, 290);
            this.trainNNButton.Name = "trainNNButton";
            this.trainNNButton.Size = new System.Drawing.Size(146, 25);
            this.trainNNButton.TabIndex = 35;
            this.trainNNButton.Text = "Train NN";
            this.trainNNButton.UseVisualStyleBackColor = true;
            this.trainNNButton.Click += new System.EventHandler(this.trainNNButton_Click);
            // 
            // createNNButton
            // 
            this.createNNButton.Location = new System.Drawing.Point(719, 259);
            this.createNNButton.Name = "createNNButton";
            this.createNNButton.Size = new System.Drawing.Size(146, 25);
            this.createNNButton.TabIndex = 36;
            this.createNNButton.Text = "Create NN";
            this.createNNButton.UseVisualStyleBackColor = true;
            this.createNNButton.Click += new System.EventHandler(this.createNNButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(577, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Labelled image path";
            // 
            // labelledImagePathTextbox
            // 
            this.labelledImagePathTextbox.Location = new System.Drawing.Point(12, 167);
            this.labelledImagePathTextbox.Name = "labelledImagePathTextbox";
            this.labelledImagePathTextbox.Size = new System.Drawing.Size(559, 20);
            this.labelledImagePathTextbox.TabIndex = 37;
            this.labelledImagePathTextbox.Text = "C:\\Emgu\\emgucv-windows-universal 3.0.0\\Emgu.CV.Example\\vtc_eugene3\\bin\\examples\\L" +
    "abeledInputs\\";
            // 
            // classifierErrorTextbox
            // 
            this.classifierErrorTextbox.Location = new System.Drawing.Point(471, 278);
            this.classifierErrorTextbox.Name = "classifierErrorTextbox";
            this.classifierErrorTextbox.Size = new System.Drawing.Size(100, 20);
            this.classifierErrorTextbox.TabIndex = 39;
            this.classifierErrorTextbox.Text = "0.001";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(577, 281);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Classifier error";
            // 
            // classifierOutputTextbox
            // 
            this.classifierOutputTextbox.Location = new System.Drawing.Point(471, 304);
            this.classifierOutputTextbox.Name = "classifierOutputTextbox";
            this.classifierOutputTextbox.Size = new System.Drawing.Size(100, 20);
            this.classifierOutputTextbox.TabIndex = 41;
            this.classifierOutputTextbox.Text = "0.001";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(577, 306);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "Classifier output";
            // 
            // exportNNButton
            // 
            this.exportNNButton.Location = new System.Drawing.Point(719, 412);
            this.exportNNButton.Name = "exportNNButton";
            this.exportNNButton.Size = new System.Drawing.Size(146, 25);
            this.exportNNButton.TabIndex = 43;
            this.exportNNButton.Text = "Export NN";
            this.exportNNButton.UseVisualStyleBackColor = true;
            // 
            // importNNButton
            // 
            this.importNNButton.Location = new System.Drawing.Point(719, 443);
            this.importNNButton.Name = "importNNButton";
            this.importNNButton.Size = new System.Drawing.Size(146, 25);
            this.importNNButton.TabIndex = 44;
            this.importNNButton.Text = "Import NN";
            this.importNNButton.UseVisualStyleBackColor = true;
            // 
            // dnnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(882, 490);
            this.Controls.Add(this.importNNButton);
            this.Controls.Add(this.exportNNButton);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.classifierOutputTextbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.classifierErrorTextbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelledImagePathTextbox);
            this.Controls.Add(this.createNNButton);
            this.Controls.Add(this.trainNNButton);
            this.Controls.Add(this.importRBMActivationsButton);
            this.Controls.Add(this.exportRBMActivationsButton);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.hiddenActivationsPathTextbox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.exportWeightsTextbox);
            this.Controls.Add(this.importWeightsButton);
            this.Controls.Add(this.exportWeightsButton);
            this.Controls.Add(this.createRBMButton);
            this.Controls.Add(this.stopTrainingButton);
            this.Controls.Add(this.startTrainingButton);
            this.Controls.Add(this.reconstructSingleButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.singleImageTextbox);
            this.Controls.Add(this.learningRateLabel);
            this.Controls.Add(this.learningRateTextbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.trainingCyclesTextbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hiddenUnitsTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.visualizationPathTextbox);
            this.Controls.Add(this.reconstructionPathTextbox);
            this.Controls.Add(this.showReconstructionsButton);
            this.Controls.Add(this.exportWeightVisButton);
            this.Controls.Add(this.trainRBMButton);
            this.Controls.Add(this.trainingPathTextbox);
            this.Name = "dnnForm";
            this.Text = "DNN Classifier Creator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox trainingPathTextbox;
        private System.Windows.Forms.Button trainRBMButton;
        private System.Windows.Forms.Button exportWeightVisButton;
        private System.Windows.Forms.Button showReconstructionsButton;
        private System.Windows.Forms.TextBox reconstructionPathTextbox;
        private System.Windows.Forms.TextBox visualizationPathTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox hiddenUnitsTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox trainingCyclesTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label learningRateLabel;
        private System.Windows.Forms.TextBox learningRateTextbox;
        private System.Windows.Forms.TextBox singleImageTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button reconstructSingleButton;
        private System.Windows.Forms.Button startTrainingButton;
        private System.Windows.Forms.Button stopTrainingButton;
        private System.Windows.Forms.Button createRBMButton;
        private System.Windows.Forms.Timer renderTimer;
        private System.Windows.Forms.Button exportWeightsButton;
        private System.Windows.Forms.Button importWeightsButton;
        private System.Windows.Forms.TextBox exportWeightsTextbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox hiddenActivationsPathTextbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button exportRBMActivationsButton;
        private System.Windows.Forms.Button importRBMActivationsButton;
        private System.Windows.Forms.Button trainNNButton;
        private System.Windows.Forms.Button createNNButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox labelledImagePathTextbox;
        private System.Windows.Forms.TextBox classifierErrorTextbox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox classifierOutputTextbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button exportNNButton;
        private System.Windows.Forms.Button importNNButton;
    }
}

