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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.selectTrainingImFolderButton = new System.Windows.Forms.Button();
            this.trainingPathTextbox = new System.Windows.Forms.TextBox();
            this.trainRBMButton = new System.Windows.Forms.Button();
            this.exportWeightVisButton = new System.Windows.Forms.Button();
            this.showReconstructionsButton = new System.Windows.Forms.Button();
            this.showDaydreamButtons = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // selectTrainingImFolderButton
            // 
            this.selectTrainingImFolderButton.Location = new System.Drawing.Point(12, 12);
            this.selectTrainingImFolderButton.Name = "selectTrainingImFolderButton";
            this.selectTrainingImFolderButton.Size = new System.Drawing.Size(170, 25);
            this.selectTrainingImFolderButton.TabIndex = 0;
            this.selectTrainingImFolderButton.Text = "Select training images folder";
            this.selectTrainingImFolderButton.UseVisualStyleBackColor = true;
            this.selectTrainingImFolderButton.Click += new System.EventHandler(this.selectTrainingImFolderButton_Click);
            // 
            // trainingPathTextbox
            // 
            this.trainingPathTextbox.Location = new System.Drawing.Point(12, 43);
            this.trainingPathTextbox.Name = "trainingPathTextbox";
            this.trainingPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.trainingPathTextbox.TabIndex = 1;
            this.trainingPathTextbox.Text = "E:\\Emgu\\emgucv-windows-universal-cuda 2.9.0.1922\\Emgu.CV.Example\\vtc_eugene3\\bin\\" +
    "examples\\Car";
            // 
            // trainRBMButton
            // 
            this.trainRBMButton.Location = new System.Drawing.Point(12, 185);
            this.trainRBMButton.Name = "trainRBMButton";
            this.trainRBMButton.Size = new System.Drawing.Size(170, 25);
            this.trainRBMButton.TabIndex = 2;
            this.trainRBMButton.Text = "Train RBM";
            this.trainRBMButton.UseVisualStyleBackColor = true;
            this.trainRBMButton.Click += new System.EventHandler(this.trainRBMButton_Click);
            // 
            // exportWeightVisButton
            // 
            this.exportWeightVisButton.Location = new System.Drawing.Point(12, 216);
            this.exportWeightVisButton.Name = "exportWeightVisButton";
            this.exportWeightVisButton.Size = new System.Drawing.Size(170, 23);
            this.exportWeightVisButton.TabIndex = 3;
            this.exportWeightVisButton.Text = "Export Weight Visualizations";
            this.exportWeightVisButton.UseVisualStyleBackColor = true;
            this.exportWeightVisButton.Click += new System.EventHandler(this.exportWeightVisButton_Click_1);
            // 
            // showReconstructionsButton
            // 
            this.showReconstructionsButton.Location = new System.Drawing.Point(12, 245);
            this.showReconstructionsButton.Name = "showReconstructionsButton";
            this.showReconstructionsButton.Size = new System.Drawing.Size(170, 23);
            this.showReconstructionsButton.TabIndex = 4;
            this.showReconstructionsButton.Text = "Show Reconstructions";
            this.showReconstructionsButton.UseVisualStyleBackColor = true;
            this.showReconstructionsButton.Click += new System.EventHandler(this.showReconstructionsButton_Click);
            // 
            // showDaydreamButtons
            // 
            this.showDaydreamButtons.Location = new System.Drawing.Point(12, 274);
            this.showDaydreamButtons.Name = "showDaydreamButtons";
            this.showDaydreamButtons.Size = new System.Drawing.Size(170, 23);
            this.showDaydreamButtons.TabIndex = 5;
            this.showDaydreamButtons.Text = "Show Daydream";
            this.showDaydreamButtons.UseVisualStyleBackColor = true;
            // 
            // reconstructionPathTextbox
            // 
            this.reconstructionPathTextbox.Location = new System.Drawing.Point(12, 69);
            this.reconstructionPathTextbox.Name = "reconstructionPathTextbox";
            this.reconstructionPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.reconstructionPathTextbox.TabIndex = 6;
            this.reconstructionPathTextbox.Text = "E:\\Emgu\\emgucv-windows-universal-cuda 2.9.0.1922\\Emgu.CV.Example\\vtc_eugene3\\bin\\" +
    "examples\\reconstructions";
            // 
            // visualizationPathTextbox
            // 
            this.visualizationPathTextbox.Location = new System.Drawing.Point(12, 95);
            this.visualizationPathTextbox.Name = "visualizationPathTextbox";
            this.visualizationPathTextbox.Size = new System.Drawing.Size(559, 20);
            this.visualizationPathTextbox.TabIndex = 7;
            this.visualizationPathTextbox.Text = "E:\\Emgu\\emgucv-windows-universal-cuda 2.9.0.1922\\Emgu.CV.Example\\vtc_eugene3\\bin\\" +
    "examples\\visualizations";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(578, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Training images path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(578, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Reconstructions path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(578, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Visualizations path";
            // 
            // hiddenUnitsTextbox
            // 
            this.hiddenUnitsTextbox.Location = new System.Drawing.Point(471, 136);
            this.hiddenUnitsTextbox.Name = "hiddenUnitsTextbox";
            this.hiddenUnitsTextbox.Size = new System.Drawing.Size(100, 20);
            this.hiddenUnitsTextbox.TabIndex = 11;
            this.hiddenUnitsTextbox.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(577, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Hidden units";
            // 
            // trainingCyclesTextbox
            // 
            this.trainingCyclesTextbox.Location = new System.Drawing.Point(471, 162);
            this.trainingCyclesTextbox.Name = "trainingCyclesTextbox";
            this.trainingCyclesTextbox.Size = new System.Drawing.Size(100, 20);
            this.trainingCyclesTextbox.TabIndex = 13;
            this.trainingCyclesTextbox.Text = "300000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(578, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Training cycles";
            // 
            // learningRateLabel
            // 
            this.learningRateLabel.AutoSize = true;
            this.learningRateLabel.Location = new System.Drawing.Point(578, 193);
            this.learningRateLabel.Name = "learningRateLabel";
            this.learningRateLabel.Size = new System.Drawing.Size(69, 13);
            this.learningRateLabel.TabIndex = 16;
            this.learningRateLabel.Text = "Learning rate";
            // 
            // learningRateTextbox
            // 
            this.learningRateTextbox.Location = new System.Drawing.Point(471, 190);
            this.learningRateTextbox.Name = "learningRateTextbox";
            this.learningRateTextbox.Size = new System.Drawing.Size(100, 20);
            this.learningRateTextbox.TabIndex = 15;
            this.learningRateTextbox.Text = "0.001";
            // 
            // dnnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 396);
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
            this.Controls.Add(this.showDaydreamButtons);
            this.Controls.Add(this.showReconstructionsButton);
            this.Controls.Add(this.exportWeightVisButton);
            this.Controls.Add(this.trainRBMButton);
            this.Controls.Add(this.trainingPathTextbox);
            this.Controls.Add(this.selectTrainingImFolderButton);
            this.Name = "dnnForm";
            this.Text = "DNN Classifier Creator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button selectTrainingImFolderButton;
        private System.Windows.Forms.TextBox trainingPathTextbox;
        private System.Windows.Forms.Button trainRBMButton;
        private System.Windows.Forms.Button exportWeightVisButton;
        private System.Windows.Forms.Button showReconstructionsButton;
        private System.Windows.Forms.Button showDaydreamButtons;
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
    }
}

