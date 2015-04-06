namespace VTC.ExportTrainingSet
{
    partial class ExportTrainingSet
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
            this.objectClassGroupBox = new System.Windows.Forms.GroupBox();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.subimagePanel = new System.Windows.Forms.Panel();
            this.infoBox = new System.Windows.Forms.TextBox();
            this.autoExportButton = new System.Windows.Forms.Button();
            this.autoExportPositives = new System.Windows.Forms.Button();
            this.exportMonoTrainingSamples = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // objectClassGroupBox
            // 
            this.objectClassGroupBox.Location = new System.Drawing.Point(12, 210);
            this.objectClassGroupBox.Name = "objectClassGroupBox";
            this.objectClassGroupBox.Size = new System.Drawing.Size(180, 282);
            this.objectClassGroupBox.TabIndex = 7;
            this.objectClassGroupBox.TabStop = false;
            this.objectClassGroupBox.Text = "Object Classification";
            // 
            // imagePanel
            // 
            this.imagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePanel.Location = new System.Drawing.Point(198, 12);
            this.imagePanel.MaximumSize = new System.Drawing.Size(640, 480);
            this.imagePanel.MinimumSize = new System.Drawing.Size(640, 480);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(640, 480);
            this.imagePanel.TabIndex = 8;
            this.imagePanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.imagePanel_MouseClick);
            // 
            // subimagePanel
            // 
            this.subimagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.subimagePanel.Location = new System.Drawing.Point(12, 12);
            this.subimagePanel.MaximumSize = new System.Drawing.Size(30, 30);
            this.subimagePanel.MinimumSize = new System.Drawing.Size(30, 30);
            this.subimagePanel.Name = "subimagePanel";
            this.subimagePanel.Size = new System.Drawing.Size(30, 30);
            this.subimagePanel.TabIndex = 9;
            // 
            // infoBox
            // 
            this.infoBox.Location = new System.Drawing.Point(13, 499);
            this.infoBox.Multiline = true;
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(179, 132);
            this.infoBox.TabIndex = 10;
            // 
            // autoExportButton
            // 
            this.autoExportButton.Location = new System.Drawing.Point(13, 49);
            this.autoExportButton.Name = "autoExportButton";
            this.autoExportButton.Size = new System.Drawing.Size(119, 37);
            this.autoExportButton.TabIndex = 11;
            this.autoExportButton.Text = "Auto Export";
            this.autoExportButton.UseVisualStyleBackColor = true;
            this.autoExportButton.Click += new System.EventHandler(this.autoExportButton_Click);
            // 
            // autoExportPositives
            // 
            this.autoExportPositives.Location = new System.Drawing.Point(13, 92);
            this.autoExportPositives.Name = "autoExportPositives";
            this.autoExportPositives.Size = new System.Drawing.Size(119, 37);
            this.autoExportPositives.TabIndex = 12;
            this.autoExportPositives.Text = "Auto Export    (positives only)";
            this.autoExportPositives.UseVisualStyleBackColor = true;
            this.autoExportPositives.Click += new System.EventHandler(this.autoExportPositives_Click);
            // 
            // exportMonoTrainingSamples
            // 
            this.exportMonoTrainingSamples.AutoSize = true;
            this.exportMonoTrainingSamples.Location = new System.Drawing.Point(13, 135);
            this.exportMonoTrainingSamples.Name = "exportMonoTrainingSamples";
            this.exportMonoTrainingSamples.Size = new System.Drawing.Size(104, 17);
            this.exportMonoTrainingSamples.TabIndex = 13;
            this.exportMonoTrainingSamples.Text = "Convert to mono";
            this.exportMonoTrainingSamples.UseVisualStyleBackColor = true;
            // 
            // ExportTrainingSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 643);
            this.Controls.Add(this.exportMonoTrainingSamples);
            this.Controls.Add(this.autoExportPositives);
            this.Controls.Add(this.autoExportButton);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.subimagePanel);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.objectClassGroupBox);
            this.Name = "ExportTrainingSet";
            this.Text = "ExportTrainingSet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox objectClassGroupBox;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.Panel subimagePanel;
        private System.Windows.Forms.TextBox infoBox;
        private System.Windows.Forms.Button autoExportButton;
        private System.Windows.Forms.Button autoExportPositives;
        private System.Windows.Forms.CheckBox exportMonoTrainingSamples;

    }
}