namespace DNNClassifier
{
    partial class DnnForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DnnForm));
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
            this.exportRBMWeightsTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
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
            this.exportImagesCheckbox = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.NNWeightsPathTextbox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.inputPictureBox = new System.Windows.Forms.PictureBox();
            this.reconstructionPictureBox = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.loadAllButton = new System.Windows.Forms.Button();
            this.calculateActivationsButton = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reconstructionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // trainingPathTextbox
            // 
            this.trainingPathTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainingPathTextbox.Location = new System.Drawing.Point(12, 12);
            this.trainingPathTextbox.Name = "trainingPathTextbox";
            this.trainingPathTextbox.Size = new System.Drawing.Size(357, 20);
            this.trainingPathTextbox.TabIndex = 1;
            this.trainingPathTextbox.Text = "\\examples\\Car";
            // 
            // trainRBMButton
            // 
            this.trainRBMButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainRBMButton.Location = new System.Drawing.Point(763, 68);
            this.trainRBMButton.Name = "trainRBMButton";
            this.trainRBMButton.Size = new System.Drawing.Size(154, 25);
            this.trainRBMButton.TabIndex = 2;
            this.trainRBMButton.Text = "Train RBM (fixed cycles)";
            this.trainRBMButton.UseVisualStyleBackColor = true;
            this.trainRBMButton.Click += new System.EventHandler(this.trainRBMButton_Click);
            // 
            // exportWeightVisButton
            // 
            this.exportWeightVisButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportWeightVisButton.Location = new System.Drawing.Point(374, 470);
            this.exportWeightVisButton.Name = "exportWeightVisButton";
            this.exportWeightVisButton.Size = new System.Drawing.Size(179, 25);
            this.exportWeightVisButton.TabIndex = 3;
            this.exportWeightVisButton.Text = "Export Weight Visualizations";
            this.exportWeightVisButton.UseVisualStyleBackColor = true;
            this.exportWeightVisButton.Click += new System.EventHandler(this.exportWeightVisButton_Click_1);
            // 
            // showReconstructionsButton
            // 
            this.showReconstructionsButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showReconstructionsButton.Location = new System.Drawing.Point(374, 439);
            this.showReconstructionsButton.Name = "showReconstructionsButton";
            this.showReconstructionsButton.Size = new System.Drawing.Size(179, 25);
            this.showReconstructionsButton.TabIndex = 4;
            this.showReconstructionsButton.Text = "Show Reconstructions";
            this.showReconstructionsButton.UseVisualStyleBackColor = true;
            this.showReconstructionsButton.Click += new System.EventHandler(this.showReconstructionsButton_Click);
            // 
            // reconstructionPathTextbox
            // 
            this.reconstructionPathTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reconstructionPathTextbox.Location = new System.Drawing.Point(12, 38);
            this.reconstructionPathTextbox.Name = "reconstructionPathTextbox";
            this.reconstructionPathTextbox.Size = new System.Drawing.Size(357, 20);
            this.reconstructionPathTextbox.TabIndex = 6;
            this.reconstructionPathTextbox.Text = "\\examples\\reconstructions";
            // 
            // visualizationPathTextbox
            // 
            this.visualizationPathTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visualizationPathTextbox.Location = new System.Drawing.Point(12, 64);
            this.visualizationPathTextbox.Name = "visualizationPathTextbox";
            this.visualizationPathTextbox.Size = new System.Drawing.Size(357, 20);
            this.visualizationPathTextbox.TabIndex = 7;
            this.visualizationPathTextbox.Text = "\\examples\\visualizations";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(376, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Training images path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(376, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Reconstructions path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(376, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Visualizations path";
            // 
            // hiddenUnitsTextbox
            // 
            this.hiddenUnitsTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hiddenUnitsTextbox.Location = new System.Drawing.Point(12, 204);
            this.hiddenUnitsTextbox.Name = "hiddenUnitsTextbox";
            this.hiddenUnitsTextbox.Size = new System.Drawing.Size(100, 20);
            this.hiddenUnitsTextbox.TabIndex = 11;
            this.hiddenUnitsTextbox.Text = "200";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(118, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(163, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "RBM hidden units/NN inputs";
            // 
            // trainingCyclesTextbox
            // 
            this.trainingCyclesTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainingCyclesTextbox.Location = new System.Drawing.Point(12, 229);
            this.trainingCyclesTextbox.Name = "trainingCyclesTextbox";
            this.trainingCyclesTextbox.Size = new System.Drawing.Size(100, 20);
            this.trainingCyclesTextbox.TabIndex = 13;
            this.trainingCyclesTextbox.Text = "5000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(119, 238);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Training cycles";
            // 
            // learningRateLabel
            // 
            this.learningRateLabel.AutoSize = true;
            this.learningRateLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.learningRateLabel.Location = new System.Drawing.Point(119, 258);
            this.learningRateLabel.Name = "learningRateLabel";
            this.learningRateLabel.Size = new System.Drawing.Size(85, 13);
            this.learningRateLabel.TabIndex = 16;
            this.learningRateLabel.Text = "Learning rate";
            // 
            // learningRateTextbox
            // 
            this.learningRateTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.learningRateTextbox.Location = new System.Drawing.Point(12, 255);
            this.learningRateTextbox.Name = "learningRateTextbox";
            this.learningRateTextbox.Size = new System.Drawing.Size(100, 20);
            this.learningRateTextbox.TabIndex = 15;
            this.learningRateTextbox.Text = "0.001";
            // 
            // singleImageTextbox
            // 
            this.singleImageTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.singleImageTextbox.Location = new System.Drawing.Point(12, 142);
            this.singleImageTextbox.Name = "singleImageTextbox";
            this.singleImageTextbox.Size = new System.Drawing.Size(357, 20);
            this.singleImageTextbox.TabIndex = 17;
            this.singleImageTextbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.singleImageTextbox_DragDrop);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(375, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Single image path";
            // 
            // reconstructSingleButton
            // 
            this.reconstructSingleButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reconstructSingleButton.Location = new System.Drawing.Point(374, 502);
            this.reconstructSingleButton.Name = "reconstructSingleButton";
            this.reconstructSingleButton.Size = new System.Drawing.Size(179, 35);
            this.reconstructSingleButton.TabIndex = 20;
            this.reconstructSingleButton.Text = "Reconstruct and classify single image";
            this.reconstructSingleButton.UseVisualStyleBackColor = true;
            this.reconstructSingleButton.Click += new System.EventHandler(this.reconstructSingleButton_Click);
            // 
            // startTrainingButton
            // 
            this.startTrainingButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startTrainingButton.Location = new System.Drawing.Point(578, 68);
            this.startTrainingButton.Name = "startTrainingButton";
            this.startTrainingButton.Size = new System.Drawing.Size(173, 25);
            this.startTrainingButton.TabIndex = 21;
            this.startTrainingButton.Text = "Start training";
            this.startTrainingButton.UseVisualStyleBackColor = true;
            this.startTrainingButton.Click += new System.EventHandler(this.startTrainingButton_Click);
            // 
            // stopTrainingButton
            // 
            this.stopTrainingButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopTrainingButton.Location = new System.Drawing.Point(578, 100);
            this.stopTrainingButton.Name = "stopTrainingButton";
            this.stopTrainingButton.Size = new System.Drawing.Size(173, 25);
            this.stopTrainingButton.TabIndex = 22;
            this.stopTrainingButton.Text = "Stop training";
            this.stopTrainingButton.UseVisualStyleBackColor = true;
            this.stopTrainingButton.Click += new System.EventHandler(this.stopTrainingButton_Click);
            // 
            // createRBMButton
            // 
            this.createRBMButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createRBMButton.Location = new System.Drawing.Point(578, 36);
            this.createRBMButton.Name = "createRBMButton";
            this.createRBMButton.Size = new System.Drawing.Size(173, 25);
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
            this.exportWeightsButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportWeightsButton.Location = new System.Drawing.Point(578, 132);
            this.exportWeightsButton.Name = "exportWeightsButton";
            this.exportWeightsButton.Size = new System.Drawing.Size(173, 25);
            this.exportWeightsButton.TabIndex = 24;
            this.exportWeightsButton.Text = "Export RBM weights";
            this.exportWeightsButton.UseVisualStyleBackColor = true;
            this.exportWeightsButton.Click += new System.EventHandler(this.exportWeightsButton_Click);
            // 
            // importWeightsButton
            // 
            this.importWeightsButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importWeightsButton.Location = new System.Drawing.Point(763, 132);
            this.importWeightsButton.Name = "importWeightsButton";
            this.importWeightsButton.Size = new System.Drawing.Size(154, 25);
            this.importWeightsButton.TabIndex = 25;
            this.importWeightsButton.Text = "Import RBM weights";
            this.importWeightsButton.UseVisualStyleBackColor = true;
            this.importWeightsButton.Click += new System.EventHandler(this.importWeightsButton_Click);
            // 
            // exportRBMWeightsTextbox
            // 
            this.exportRBMWeightsTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportRBMWeightsTextbox.Location = new System.Drawing.Point(12, 90);
            this.exportRBMWeightsTextbox.Name = "exportRBMWeightsTextbox";
            this.exportRBMWeightsTextbox.Size = new System.Drawing.Size(357, 20);
            this.exportRBMWeightsTextbox.TabIndex = 26;
            this.exportRBMWeightsTextbox.Text = "\\Weights\\rbm_weights.bin";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(375, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "RBM Weights  path";
            // 
            // trainNNButton
            // 
            this.trainNNButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainNNButton.Location = new System.Drawing.Point(578, 228);
            this.trainNNButton.Name = "trainNNButton";
            this.trainNNButton.Size = new System.Drawing.Size(173, 25);
            this.trainNNButton.TabIndex = 35;
            this.trainNNButton.Text = "Train NN";
            this.trainNNButton.UseVisualStyleBackColor = true;
            this.trainNNButton.Click += new System.EventHandler(this.trainNNButton_Click);
            // 
            // createNNButton
            // 
            this.createNNButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createNNButton.Location = new System.Drawing.Point(578, 164);
            this.createNNButton.Name = "createNNButton";
            this.createNNButton.Size = new System.Drawing.Size(173, 25);
            this.createNNButton.TabIndex = 36;
            this.createNNButton.Text = "Create NN";
            this.createNNButton.UseVisualStyleBackColor = true;
            this.createNNButton.Click += new System.EventHandler(this.createNNButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(375, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(121, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Labelled image path";
            // 
            // labelledImagePathTextbox
            // 
            this.labelledImagePathTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelledImagePathTextbox.Location = new System.Drawing.Point(12, 168);
            this.labelledImagePathTextbox.Name = "labelledImagePathTextbox";
            this.labelledImagePathTextbox.Size = new System.Drawing.Size(357, 20);
            this.labelledImagePathTextbox.TabIndex = 37;
            this.labelledImagePathTextbox.Text = "\\examples\\LabeledInputs\\";
            // 
            // classifierErrorTextbox
            // 
            this.classifierErrorTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.classifierErrorTextbox.Location = new System.Drawing.Point(12, 281);
            this.classifierErrorTextbox.Name = "classifierErrorTextbox";
            this.classifierErrorTextbox.Size = new System.Drawing.Size(100, 20);
            this.classifierErrorTextbox.TabIndex = 39;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(118, 284);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Classifier error";
            // 
            // classifierOutputTextbox
            // 
            this.classifierOutputTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.classifierOutputTextbox.Location = new System.Drawing.Point(12, 307);
            this.classifierOutputTextbox.Name = "classifierOutputTextbox";
            this.classifierOutputTextbox.Size = new System.Drawing.Size(100, 20);
            this.classifierOutputTextbox.TabIndex = 41;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(118, 309);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(109, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "Classifier output";
            // 
            // exportNNButton
            // 
            this.exportNNButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportNNButton.Location = new System.Drawing.Point(578, 260);
            this.exportNNButton.Name = "exportNNButton";
            this.exportNNButton.Size = new System.Drawing.Size(173, 25);
            this.exportNNButton.TabIndex = 43;
            this.exportNNButton.Text = "Export NN";
            this.exportNNButton.UseVisualStyleBackColor = true;
            this.exportNNButton.Click += new System.EventHandler(this.exportNNButton_Click);
            // 
            // importNNButton
            // 
            this.importNNButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importNNButton.Location = new System.Drawing.Point(763, 163);
            this.importNNButton.Name = "importNNButton";
            this.importNNButton.Size = new System.Drawing.Size(154, 25);
            this.importNNButton.TabIndex = 44;
            this.importNNButton.Text = "Import NN";
            this.importNNButton.UseVisualStyleBackColor = true;
            this.importNNButton.Click += new System.EventHandler(this.importNNButton_Click);
            // 
            // exportImagesCheckbox
            // 
            this.exportImagesCheckbox.AutoSize = true;
            this.exportImagesCheckbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportImagesCheckbox.Location = new System.Drawing.Point(12, 333);
            this.exportImagesCheckbox.Name = "exportImagesCheckbox";
            this.exportImagesCheckbox.Size = new System.Drawing.Size(350, 17);
            this.exportImagesCheckbox.TabIndex = 45;
            this.exportImagesCheckbox.Text = "Export weight visualizations and image reconstructions";
            this.exportImagesCheckbox.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(375, 116);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 13);
            this.label12.TabIndex = 47;
            this.label12.Text = "NN Weights  path";
            // 
            // NNWeightsPathTextbox
            // 
            this.NNWeightsPathTextbox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NNWeightsPathTextbox.Location = new System.Drawing.Point(12, 116);
            this.NNWeightsPathTextbox.Name = "NNWeightsPathTextbox";
            this.NNWeightsPathTextbox.Size = new System.Drawing.Size(357, 20);
            this.NNWeightsPathTextbox.TabIndex = 46;
            this.NNWeightsPathTextbox.Text = "\\Weights\\nn_weights.bin";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(597, 380);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(320, 159);
            this.textBox1.TabIndex = 48;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // inputPictureBox
            // 
            this.inputPictureBox.BackColor = System.Drawing.Color.White;
            this.inputPictureBox.Location = new System.Drawing.Point(12, 380);
            this.inputPictureBox.Name = "inputPictureBox";
            this.inputPictureBox.Size = new System.Drawing.Size(175, 159);
            this.inputPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.inputPictureBox.TabIndex = 49;
            this.inputPictureBox.TabStop = false;
            // 
            // reconstructionPictureBox
            // 
            this.reconstructionPictureBox.BackColor = System.Drawing.Color.White;
            this.reconstructionPictureBox.Location = new System.Drawing.Point(193, 380);
            this.reconstructionPictureBox.Name = "reconstructionPictureBox";
            this.reconstructionPictureBox.Size = new System.Drawing.Size(175, 159);
            this.reconstructionPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.reconstructionPictureBox.TabIndex = 50;
            this.reconstructionPictureBox.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(11, 361);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 13);
            this.label13.TabIndex = 51;
            this.label13.Text = "Input";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(190, 361);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(91, 13);
            this.label14.TabIndex = 52;
            this.label14.Text = "Reconstruction";
            // 
            // loadAllButton
            // 
            this.loadAllButton.BackColor = System.Drawing.SystemColors.ControlDark;
            this.loadAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.loadAllButton.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadAllButton.ForeColor = System.Drawing.Color.Black;
            this.loadAllButton.Location = new System.Drawing.Point(763, 238);
            this.loadAllButton.Name = "loadAllButton";
            this.loadAllButton.Size = new System.Drawing.Size(154, 45);
            this.loadAllButton.TabIndex = 53;
            this.loadAllButton.Text = "Load All";
            this.loadAllButton.UseVisualStyleBackColor = false;
            this.loadAllButton.Click += new System.EventHandler(this.loadAllButton_Click);
            // 
            // calculateActivationsButton
            // 
            this.calculateActivationsButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calculateActivationsButton.Location = new System.Drawing.Point(578, 196);
            this.calculateActivationsButton.Name = "calculateActivationsButton";
            this.calculateActivationsButton.Size = new System.Drawing.Size(173, 25);
            this.calculateActivationsButton.TabIndex = 54;
            this.calculateActivationsButton.Text = "Calculate RBM activations";
            this.calculateActivationsButton.UseVisualStyleBackColor = true;
            this.calculateActivationsButton.Click += new System.EventHandler(this.calculateActivationsButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(592, 5);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(220, 28);
            this.label15.TabIndex = 55;
            this.label15.Text = "Create and Train";
            // 
            // DnnForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(931, 548);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.calculateActivationsButton);
            this.Controls.Add(this.loadAllButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.reconstructionPictureBox);
            this.Controls.Add(this.inputPictureBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.NNWeightsPathTextbox);
            this.Controls.Add(this.exportImagesCheckbox);
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
            this.Controls.Add(this.label7);
            this.Controls.Add(this.exportRBMWeightsTextbox);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DnnForm";
            this.Text = "DNN Classifier Creator";
            ((System.ComponentModel.ISupportInitialize)(this.inputPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reconstructionPictureBox)).EndInit();
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
        private System.Windows.Forms.TextBox exportRBMWeightsTextbox;
        private System.Windows.Forms.Label label7;
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
        private System.Windows.Forms.CheckBox exportImagesCheckbox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox NNWeightsPathTextbox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox inputPictureBox;
        private System.Windows.Forms.PictureBox reconstructionPictureBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button loadAllButton;
        private System.Windows.Forms.Button calculateActivationsButton;
        private System.Windows.Forms.Label label15;
    }
}

