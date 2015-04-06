namespace ViolaJonesTrainer
{
    partial class ViolaJonesForm
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
            this.classesComboBox = new System.Windows.Forms.ComboBox();
            this.positiveExampleImages = new System.Windows.Forms.Panel();
            this.negativeExampleImages = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trainButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.infoBox = new System.Windows.Forms.TextBox();
            this.validationImagePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.validationImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // classesComboBox
            // 
            this.classesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.classesComboBox.FormattingEnabled = true;
            this.classesComboBox.Location = new System.Drawing.Point(12, 12);
            this.classesComboBox.Name = "classesComboBox";
            this.classesComboBox.Size = new System.Drawing.Size(576, 21);
            this.classesComboBox.TabIndex = 0;
            this.classesComboBox.SelectedIndexChanged += new System.EventHandler(this.classesComboBox_SelectedIndexChanged);
            // 
            // positiveExampleImages
            // 
            this.positiveExampleImages.Location = new System.Drawing.Point(12, 56);
            this.positiveExampleImages.Name = "positiveExampleImages";
            this.positiveExampleImages.Size = new System.Drawing.Size(789, 100);
            this.positiveExampleImages.TabIndex = 1;
            // 
            // negativeExampleImages
            // 
            this.negativeExampleImages.Location = new System.Drawing.Point(12, 196);
            this.negativeExampleImages.Name = "negativeExampleImages";
            this.negativeExampleImages.Size = new System.Drawing.Size(789, 100);
            this.negativeExampleImages.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Positive examples";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Negative examples";
            // 
            // trainButton
            // 
            this.trainButton.Location = new System.Drawing.Point(862, 56);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(138, 41);
            this.trainButton.TabIndex = 7;
            this.trainButton.Text = "Train";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.trainButton_Click);
            // 
            // validateButton
            // 
            this.validateButton.Location = new System.Drawing.Point(862, 103);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(138, 41);
            this.validateButton.TabIndex = 8;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // infoBox
            // 
            this.infoBox.Location = new System.Drawing.Point(862, 172);
            this.infoBox.Multiline = true;
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(187, 266);
            this.infoBox.TabIndex = 9;
            // 
            // validationImagePictureBox
            // 
            this.validationImagePictureBox.Location = new System.Drawing.Point(12, 303);
            this.validationImagePictureBox.Name = "validationImagePictureBox";
            this.validationImagePictureBox.Size = new System.Drawing.Size(100, 50);
            this.validationImagePictureBox.TabIndex = 10;
            this.validationImagePictureBox.TabStop = false;
            // 
            // ViolaJonesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 843);
            this.Controls.Add(this.validationImagePictureBox);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.trainButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.negativeExampleImages);
            this.Controls.Add(this.positiveExampleImages);
            this.Controls.Add(this.classesComboBox);
            this.Name = "ViolaJonesForm";
            this.Text = "Viola Jones trainer";
            ((System.ComponentModel.ISupportInitialize)(this.validationImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox classesComboBox;
        private System.Windows.Forms.Panel positiveExampleImages;
        private System.Windows.Forms.Panel negativeExampleImages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button trainButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.TextBox infoBox;
        private System.Windows.Forms.PictureBox validationImagePictureBox;
    }
}

