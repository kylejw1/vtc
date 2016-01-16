namespace CUDATests
{
    partial class Form1
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
            this.frame1Picturebox = new System.Windows.Forms.PictureBox();
            this.frame2Picturebox = new System.Windows.Forms.PictureBox();
            this.resultPicturebox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.callDLLButton = new System.Windows.Forms.Button();
            this.maxShiftTextbox = new System.Windows.Forms.TextBox();
            this.apertureTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.frame1Textbox = new System.Windows.Forms.TextBox();
            this.frame2Textbox = new System.Windows.Forms.TextBox();
            this.outputTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.frame1Picturebox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frame2Picturebox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultPicturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // frame1Picturebox
            // 
            this.frame1Picturebox.BackColor = System.Drawing.Color.White;
            this.frame1Picturebox.Location = new System.Drawing.Point(12, 133);
            this.frame1Picturebox.Name = "frame1Picturebox";
            this.frame1Picturebox.Size = new System.Drawing.Size(258, 187);
            this.frame1Picturebox.TabIndex = 0;
            this.frame1Picturebox.TabStop = false;
            // 
            // frame2Picturebox
            // 
            this.frame2Picturebox.BackColor = System.Drawing.Color.White;
            this.frame2Picturebox.Location = new System.Drawing.Point(276, 133);
            this.frame2Picturebox.Name = "frame2Picturebox";
            this.frame2Picturebox.Size = new System.Drawing.Size(258, 187);
            this.frame2Picturebox.TabIndex = 1;
            this.frame2Picturebox.TabStop = false;
            // 
            // resultPicturebox
            // 
            this.resultPicturebox.BackColor = System.Drawing.Color.White;
            this.resultPicturebox.Location = new System.Drawing.Point(540, 133);
            this.resultPicturebox.Name = "resultPicturebox";
            this.resultPicturebox.Size = new System.Drawing.Size(258, 187);
            this.resultPicturebox.TabIndex = 2;
            this.resultPicturebox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "frame 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(273, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "frame 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(540, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "result";
            // 
            // callDLLButton
            // 
            this.callDLLButton.Location = new System.Drawing.Point(12, 12);
            this.callDLLButton.Name = "callDLLButton";
            this.callDLLButton.Size = new System.Drawing.Size(100, 23);
            this.callDLLButton.TabIndex = 6;
            this.callDLLButton.Text = "Call DLL";
            this.callDLLButton.UseVisualStyleBackColor = true;
            this.callDLLButton.Click += new System.EventHandler(this.callDLLButton_Click);
            // 
            // maxShiftTextbox
            // 
            this.maxShiftTextbox.Location = new System.Drawing.Point(12, 41);
            this.maxShiftTextbox.Name = "maxShiftTextbox";
            this.maxShiftTextbox.Size = new System.Drawing.Size(100, 20);
            this.maxShiftTextbox.TabIndex = 7;
            this.maxShiftTextbox.Text = "5";
            // 
            // apertureTextbox
            // 
            this.apertureTextbox.Location = new System.Drawing.Point(118, 41);
            this.apertureTextbox.Name = "apertureTextbox";
            this.apertureTextbox.Size = new System.Drawing.Size(100, 20);
            this.apertureTextbox.TabIndex = 8;
            this.apertureTextbox.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Max-shift";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Aperture";
            // 
            // frame1Textbox
            // 
            this.frame1Textbox.Location = new System.Drawing.Point(64, 107);
            this.frame1Textbox.Name = "frame1Textbox";
            this.frame1Textbox.Size = new System.Drawing.Size(206, 20);
            this.frame1Textbox.TabIndex = 12;
            this.frame1Textbox.Text = "C:\\images\\frame1.png";
            // 
            // frame2Textbox
            // 
            this.frame2Textbox.Location = new System.Drawing.Point(328, 107);
            this.frame2Textbox.Name = "frame2Textbox";
            this.frame2Textbox.Size = new System.Drawing.Size(206, 20);
            this.frame2Textbox.TabIndex = 13;
            this.frame2Textbox.Text = "C:\\images\\frame2.png";
            // 
            // outputTextbox
            // 
            this.outputTextbox.Location = new System.Drawing.Point(328, 12);
            this.outputTextbox.Name = "outputTextbox";
            this.outputTextbox.Size = new System.Drawing.Size(206, 20);
            this.outputTextbox.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(283, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Output";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 340);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.outputTextbox);
            this.Controls.Add(this.frame2Textbox);
            this.Controls.Add(this.frame1Textbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.apertureTextbox);
            this.Controls.Add(this.maxShiftTextbox);
            this.Controls.Add(this.callDLLButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.resultPicturebox);
            this.Controls.Add(this.frame2Picturebox);
            this.Controls.Add(this.frame1Picturebox);
            this.Name = "Form1";
            this.Text = "CUDA Test";
            ((System.ComponentModel.ISupportInitialize)(this.frame1Picturebox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frame2Picturebox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultPicturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox frame1Picturebox;
        private System.Windows.Forms.PictureBox frame2Picturebox;
        private System.Windows.Forms.PictureBox resultPicturebox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button callDLLButton;
        private System.Windows.Forms.TextBox maxShiftTextbox;
        private System.Windows.Forms.TextBox apertureTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox frame1Textbox;
        private System.Windows.Forms.TextBox frame2Textbox;
        private System.Windows.Forms.TextBox outputTextbox;
        private System.Windows.Forms.Label label6;
    }
}

