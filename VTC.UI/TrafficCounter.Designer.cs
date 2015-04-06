using System.Windows.Forms;

namespace VTC
{
   partial class TrafficCounter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrafficCounter));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.exportTrainingImagesButton = new System.Windows.Forms.Button();
            this.timeActiveTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.frameHeightBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.frameWidthBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.serverUrlTextBox = new System.Windows.Forms.TextBox();
            this.debugTextBox = new System.Windows.Forms.TextBox();
            this.tbVistaStats = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.intersectionIDTextBox = new System.Windows.Forms.TextBox();
            this.SaveParametersBtn = new System.Windows.Forms.Button();
            this.CameraComboBox = new System.Windows.Forms.ComboBox();
            this.btnConfigureRegions = new System.Windows.Forms.Button();
            this.showPolygonsCheckbox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.trackCountBox = new System.Windows.Forms.TextBox();
            this.pushStateCheckbox = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.imageBox3 = new Emgu.CV.UI.ImageBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.movementMaskBox = new Emgu.CV.UI.ImageBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pushStateTimer = new System.Windows.Forms.Timer(this.components);
            this.rgbCoordinateTextbox = new System.Windows.Forms.TextBox();
            this.saveRGBSamplesCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.movementMaskBox)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1073, 731);
            this.splitContainer1.SplitterDistance = 536;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.imageBox1);
            this.splitContainer3.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.saveRGBSamplesCheckBox);
            this.splitContainer3.Panel2.Controls.Add(this.rgbCoordinateTextbox);
            this.splitContainer3.Panel2.Controls.Add(this.exportTrainingImagesButton);
            this.splitContainer3.Panel2.Controls.Add(this.timeActiveTextBox);
            this.splitContainer3.Panel2.Controls.Add(this.label11);
            this.splitContainer3.Panel2.Controls.Add(this.button1);
            this.splitContainer3.Panel2.Controls.Add(this.frameHeightBox);
            this.splitContainer3.Panel2.Controls.Add(this.label6);
            this.splitContainer3.Panel2.Controls.Add(this.label5);
            this.splitContainer3.Panel2.Controls.Add(this.frameWidthBox);
            this.splitContainer3.Panel2.Controls.Add(this.label7);
            this.splitContainer3.Panel2.Controls.Add(this.serverUrlTextBox);
            this.splitContainer3.Panel2.Controls.Add(this.debugTextBox);
            this.splitContainer3.Panel2.Controls.Add(this.tbVistaStats);
            this.splitContainer3.Panel2.Controls.Add(this.label4);
            this.splitContainer3.Panel2.Controls.Add(this.intersectionIDTextBox);
            this.splitContainer3.Panel2.Controls.Add(this.SaveParametersBtn);
            this.splitContainer3.Panel2.Controls.Add(this.CameraComboBox);
            this.splitContainer3.Panel2.Controls.Add(this.btnConfigureRegions);
            this.splitContainer3.Panel2.Controls.Add(this.showPolygonsCheckbox);
            this.splitContainer3.Panel2.Controls.Add(this.label10);
            this.splitContainer3.Panel2.Controls.Add(this.trackCountBox);
            this.splitContainer3.Panel2.Controls.Add(this.pushStateCheckbox);
            this.splitContainer3.Size = new System.Drawing.Size(536, 731);
            this.splitContainer3.SplitterDistance = 365;
            this.splitContainer3.TabIndex = 0;
            // 
            // imageBox1
            // 
            this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox1.Location = new System.Drawing.Point(0, 20);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(536, 345);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            this.imageBox1.Click += new System.EventHandler(this.imageBox1_Click);
            this.imageBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageBox1_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(536, 20);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Camera ";
            // 
            // exportTrainingImagesButton
            // 
            this.exportTrainingImagesButton.Location = new System.Drawing.Point(256, 122);
            this.exportTrainingImagesButton.Name = "exportTrainingImagesButton";
            this.exportTrainingImagesButton.Size = new System.Drawing.Size(196, 23);
            this.exportTrainingImagesButton.TabIndex = 42;
            this.exportTrainingImagesButton.Text = "Export training images";
            this.exportTrainingImagesButton.UseVisualStyleBackColor = true;
            this.exportTrainingImagesButton.Click += new System.EventHandler(this.exportTrainingImagesButton_Click);
            // 
            // timeActiveTextBox
            // 
            this.timeActiveTextBox.Location = new System.Drawing.Point(6, 323);
            this.timeActiveTextBox.Name = "timeActiveTextBox";
            this.timeActiveTextBox.ReadOnly = true;
            this.timeActiveTextBox.Size = new System.Drawing.Size(100, 20);
            this.timeActiveTextBox.TabIndex = 41;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(112, 326);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "Time active";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 320);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(196, 23);
            this.button1.TabIndex = 39;
            this.button1.Text = "Resample background";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.resampleBackgroundButton_Click);
            // 
            // frameHeightBox
            // 
            this.frameHeightBox.Location = new System.Drawing.Point(359, 36);
            this.frameHeightBox.Name = "frameHeightBox";
            this.frameHeightBox.Size = new System.Drawing.Size(37, 20);
            this.frameHeightBox.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(402, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Height (px)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(298, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Width (px)";
            // 
            // frameWidthBox
            // 
            this.frameWidthBox.Location = new System.Drawing.Point(257, 36);
            this.frameWidthBox.Name = "frameWidthBox";
            this.frameWidthBox.Size = new System.Drawing.Size(37, 20);
            this.frameWidthBox.TabIndex = 34;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(452, 235);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Server URL";
            // 
            // serverUrlTextBox
            // 
            this.serverUrlTextBox.Location = new System.Drawing.Point(257, 232);
            this.serverUrlTextBox.Name = "serverUrlTextBox";
            this.serverUrlTextBox.Size = new System.Drawing.Size(195, 20);
            this.serverUrlTextBox.TabIndex = 32;
            // 
            // debugTextBox
            // 
            this.debugTextBox.Location = new System.Drawing.Point(9, 122);
            this.debugTextBox.Multiline = true;
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(240, 112);
            this.debugTextBox.TabIndex = 31;
            // 
            // tbVistaStats
            // 
            this.tbVistaStats.Location = new System.Drawing.Point(9, 20);
            this.tbVistaStats.Multiline = true;
            this.tbVistaStats.Name = "tbVistaStats";
            this.tbVistaStats.ReadOnly = true;
            this.tbVistaStats.Size = new System.Drawing.Size(240, 96);
            this.tbVistaStats.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(354, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Intersection ID";
            // 
            // intersectionIDTextBox
            // 
            this.intersectionIDTextBox.Location = new System.Drawing.Point(256, 206);
            this.intersectionIDTextBox.Name = "intersectionIDTextBox";
            this.intersectionIDTextBox.Size = new System.Drawing.Size(92, 20);
            this.intersectionIDTextBox.TabIndex = 27;
            // 
            // SaveParametersBtn
            // 
            this.SaveParametersBtn.Location = new System.Drawing.Point(256, 291);
            this.SaveParametersBtn.Name = "SaveParametersBtn";
            this.SaveParametersBtn.Size = new System.Drawing.Size(196, 23);
            this.SaveParametersBtn.TabIndex = 26;
            this.SaveParametersBtn.Text = "Save Parameters";
            this.SaveParametersBtn.UseVisualStyleBackColor = true;
            this.SaveParametersBtn.Click += new System.EventHandler(this.SaveParametersBtn_Click);
            // 
            // CameraComboBox
            // 
            this.CameraComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CameraComboBox.FormattingEnabled = true;
            this.CameraComboBox.Location = new System.Drawing.Point(256, 9);
            this.CameraComboBox.Name = "CameraComboBox";
            this.CameraComboBox.Size = new System.Drawing.Size(196, 21);
            this.CameraComboBox.TabIndex = 25;
            this.CameraComboBox.SelectedIndexChanged += new System.EventHandler(this.CameraComboBox_SelectedIndexChanged);
            // 
            // btnConfigureRegions
            // 
            this.btnConfigureRegions.Location = new System.Drawing.Point(256, 91);
            this.btnConfigureRegions.Name = "btnConfigureRegions";
            this.btnConfigureRegions.Size = new System.Drawing.Size(196, 23);
            this.btnConfigureRegions.TabIndex = 24;
            this.btnConfigureRegions.Text = "Configure Regions";
            this.btnConfigureRegions.UseVisualStyleBackColor = true;
            this.btnConfigureRegions.Click += new System.EventHandler(this.btnConfigureRegions_Click);
            // 
            // showPolygonsCheckbox
            // 
            this.showPolygonsCheckbox.AutoSize = true;
            this.showPolygonsCheckbox.Location = new System.Drawing.Point(257, 68);
            this.showPolygonsCheckbox.Name = "showPolygonsCheckbox";
            this.showPolygonsCheckbox.Size = new System.Drawing.Size(98, 17);
            this.showPolygonsCheckbox.TabIndex = 23;
            this.showPolygonsCheckbox.Text = "Show polygons";
            this.showPolygonsCheckbox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(354, 183);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Objects tracked";
            // 
            // trackCountBox
            // 
            this.trackCountBox.Location = new System.Drawing.Point(257, 180);
            this.trackCountBox.Name = "trackCountBox";
            this.trackCountBox.Size = new System.Drawing.Size(91, 20);
            this.trackCountBox.TabIndex = 21;
            // 
            // pushStateCheckbox
            // 
            this.pushStateCheckbox.AutoSize = true;
            this.pushStateCheckbox.Location = new System.Drawing.Point(257, 258);
            this.pushStateCheckbox.Name = "pushStateCheckbox";
            this.pushStateCheckbox.Size = new System.Drawing.Size(117, 17);
            this.pushStateCheckbox.TabIndex = 20;
            this.pushStateCheckbox.Text = "Push state samples";
            this.pushStateCheckbox.UseVisualStyleBackColor = true;
            this.pushStateCheckbox.CheckedChanged += new System.EventHandler(this.pushStateCheckbox_CheckedChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.movementMaskBox);
            this.splitContainer2.Panel2.Controls.Add(this.panel3);
            this.splitContainer2.Size = new System.Drawing.Size(533, 731);
            this.splitContainer2.SplitterDistance = 365;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 20);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.imageBox2);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.imageBox3);
            this.splitContainer4.Size = new System.Drawing.Size(533, 345);
            this.splitContainer4.SplitterDistance = 266;
            this.splitContainer4.TabIndex = 11;
            // 
            // imageBox2
            // 
            this.imageBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox2.Location = new System.Drawing.Point(0, 0);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(266, 345);
            this.imageBox2.TabIndex = 11;
            this.imageBox2.TabStop = false;
            // 
            // imageBox3
            // 
            this.imageBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox3.Location = new System.Drawing.Point(0, 0);
            this.imageBox3.Name = "imageBox3";
            this.imageBox3.Size = new System.Drawing.Size(263, 345);
            this.imageBox3.TabIndex = 12;
            this.imageBox3.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(533, 20);
            this.panel2.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Background";
            // 
            // movementMaskBox
            // 
            this.movementMaskBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movementMaskBox.Location = new System.Drawing.Point(0, 20);
            this.movementMaskBox.Name = "movementMaskBox";
            this.movementMaskBox.Size = new System.Drawing.Size(533, 342);
            this.movementMaskBox.TabIndex = 6;
            this.movementMaskBox.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(533, 20);
            this.panel3.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Movement ";
            // 
            // pushStateTimer
            // 
            this.pushStateTimer.Enabled = true;
            this.pushStateTimer.Interval = 10000;
            this.pushStateTimer.Tick += new System.EventHandler(this.PushStateProcess);
            // 
            // rgbCoordinateTextbox
            // 
            this.rgbCoordinateTextbox.Location = new System.Drawing.Point(6, 258);
            this.rgbCoordinateTextbox.Name = "rgbCoordinateTextbox";
            this.rgbCoordinateTextbox.Size = new System.Drawing.Size(100, 20);
            this.rgbCoordinateTextbox.TabIndex = 43;
            // 
            // saveRGBSamplesCheckBox
            // 
            this.saveRGBSamplesCheckBox.AutoSize = true;
            this.saveRGBSamplesCheckBox.Location = new System.Drawing.Point(10, 285);
            this.saveRGBSamplesCheckBox.Name = "saveRGBSamplesCheckBox";
            this.saveRGBSamplesCheckBox.Size = new System.Drawing.Size(87, 17);
            this.saveRGBSamplesCheckBox.TabIndex = 44;
            this.saveRGBSamplesCheckBox.Text = "Sample RGB";
            this.saveRGBSamplesCheckBox.UseVisualStyleBackColor = true;
            // 
            // TrafficCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 734);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrafficCounter";
            this.Text = "VTC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrafficCounter_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.movementMaskBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.Panel panel1;
      private Emgu.CV.UI.ImageBox imageBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.SplitContainer splitContainer2;
      private System.Windows.Forms.SplitContainer splitContainer3;
      private Emgu.CV.UI.ImageBox movementMaskBox;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.CheckBox pushStateCheckbox;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.TextBox trackCountBox;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.CheckBox showPolygonsCheckbox;
      private System.Windows.Forms.Timer pushStateTimer;
      private System.Windows.Forms.Button btnConfigureRegions;
      private System.Windows.Forms.ComboBox CameraComboBox;
      private System.Windows.Forms.Button SaveParametersBtn;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox intersectionIDTextBox;
      private System.Windows.Forms.TextBox tbVistaStats;
      private System.Windows.Forms.TextBox debugTextBox;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.TextBox serverUrlTextBox;
      private System.Windows.Forms.TextBox frameHeightBox;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox frameWidthBox;
      private Button button1;
      private TextBox timeActiveTextBox;
      private Label label11;
      private Button exportTrainingImagesButton;
      private SplitContainer splitContainer4;
      private Emgu.CV.UI.ImageBox imageBox2;
      private Emgu.CV.UI.ImageBox imageBox3;
      private CheckBox saveRGBSamplesCheckBox;
      private TextBox rgbCoordinateTextbox;
   }
}