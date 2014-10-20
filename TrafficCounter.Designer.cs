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
          this.debugTextBox = new System.Windows.Forms.TextBox();
          this.label7 = new System.Windows.Forms.Label();
          this.configPathBox = new System.Windows.Forms.TextBox();
          this.label4 = new System.Windows.Forms.Label();
          this.intersectionIDTextBox = new System.Windows.Forms.TextBox();
          this.SaveParametersBtn = new System.Windows.Forms.Button();
          this.CameraComboBox = new System.Windows.Forms.ComboBox();
          this.btnConfigureRegions = new System.Windows.Forms.Button();
          this.showPolygonsCheckbox = new System.Windows.Forms.CheckBox();
          this.label10 = new System.Windows.Forms.Label();
          this.trackCountBox = new System.Windows.Forms.TextBox();
          this.pushStateCheckbox = new System.Windows.Forms.CheckBox();
          this.label9 = new System.Windows.Forms.Label();
          this.avgNoiseTextbox = new System.Windows.Forms.TextBox();
          this.label8 = new System.Windows.Forms.Label();
          this.avgAreaTextbox = new System.Windows.Forms.TextBox();
          this.label6 = new System.Windows.Forms.Label();
          this.movementMassBox = new System.Windows.Forms.TextBox();
          this.label5 = new System.Windows.Forms.Label();
          this.detectionCountBox = new System.Windows.Forms.TextBox();
          this.splitContainer2 = new System.Windows.Forms.SplitContainer();
          this.imageBox2 = new Emgu.CV.UI.ImageBox();
          this.panel2 = new System.Windows.Forms.Panel();
          this.label2 = new System.Windows.Forms.Label();
          this.imageBox3 = new Emgu.CV.UI.ImageBox();
          this.panel3 = new System.Windows.Forms.Panel();
          this.label3 = new System.Windows.Forms.Label();
          this.pushStateTimer = new System.Windows.Forms.Timer(this.components);
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
          ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
          this.panel2.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).BeginInit();
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
          this.splitContainer1.Size = new System.Drawing.Size(1051, 684);
          this.splitContainer1.SplitterDistance = 517;
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
          this.splitContainer3.Panel2.Controls.Add(this.debugTextBox);
          this.splitContainer3.Panel2.Controls.Add(this.label7);
          this.splitContainer3.Panel2.Controls.Add(this.configPathBox);
          this.splitContainer3.Panel2.Controls.Add(this.label4);
          this.splitContainer3.Panel2.Controls.Add(this.intersectionIDTextBox);
          this.splitContainer3.Panel2.Controls.Add(this.SaveParametersBtn);
          this.splitContainer3.Panel2.Controls.Add(this.CameraComboBox);
          this.splitContainer3.Panel2.Controls.Add(this.btnConfigureRegions);
          this.splitContainer3.Panel2.Controls.Add(this.showPolygonsCheckbox);
          this.splitContainer3.Panel2.Controls.Add(this.label10);
          this.splitContainer3.Panel2.Controls.Add(this.trackCountBox);
          this.splitContainer3.Panel2.Controls.Add(this.pushStateCheckbox);
          this.splitContainer3.Panel2.Controls.Add(this.label9);
          this.splitContainer3.Panel2.Controls.Add(this.avgNoiseTextbox);
          this.splitContainer3.Panel2.Controls.Add(this.label8);
          this.splitContainer3.Panel2.Controls.Add(this.avgAreaTextbox);
          this.splitContainer3.Panel2.Controls.Add(this.label6);
          this.splitContainer3.Panel2.Controls.Add(this.movementMassBox);
          this.splitContainer3.Panel2.Controls.Add(this.label5);
          this.splitContainer3.Panel2.Controls.Add(this.detectionCountBox);
          this.splitContainer3.Size = new System.Drawing.Size(517, 684);
          this.splitContainer3.SplitterDistance = 342;
          this.splitContainer3.TabIndex = 0;
          // 
          // imageBox1
          // 
          this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.imageBox1.Location = new System.Drawing.Point(0, 20);
          this.imageBox1.Name = "imageBox1";
          this.imageBox1.Size = new System.Drawing.Size(517, 322);
          this.imageBox1.TabIndex = 2;
          this.imageBox1.TabStop = false;
          this.imageBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageBox1_MouseDown);
          // 
          // panel1
          // 
          this.panel1.Controls.Add(this.label1);
          this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
          this.panel1.Location = new System.Drawing.Point(0, 0);
          this.panel1.Name = "panel1";
          this.panel1.Size = new System.Drawing.Size(517, 20);
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
          // debugTextBox
          // 
          this.debugTextBox.Location = new System.Drawing.Point(9, 9);
          this.debugTextBox.Multiline = true;
          this.debugTextBox.Name = "debugTextBox";
          this.debugTextBox.Size = new System.Drawing.Size(240, 279);
          this.debugTextBox.TabIndex = 31;
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(352, 297);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(109, 13);
          this.label7.TabIndex = 30;
          this.label7.Text = "Configuration file path";
          // 
          // configPathBox
          // 
          this.configPathBox.Location = new System.Drawing.Point(9, 294);
          this.configPathBox.Multiline = true;
          this.configPathBox.Name = "configPathBox";
          this.configPathBox.Size = new System.Drawing.Size(338, 41);
          this.configPathBox.TabIndex = 29;
          // 
          // label4
          // 
          this.label4.AutoSize = true;
          this.label4.Location = new System.Drawing.Point(352, 273);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(76, 13);
          this.label4.TabIndex = 28;
          this.label4.Text = "Intersection ID";
          // 
          // intersectionIDTextBox
          // 
          this.intersectionIDTextBox.Location = new System.Drawing.Point(255, 270);
          this.intersectionIDTextBox.Name = "intersectionIDTextBox";
          this.intersectionIDTextBox.Size = new System.Drawing.Size(92, 20);
          this.intersectionIDTextBox.TabIndex = 27;
          // 
          // SaveParametersBtn
          // 
          this.SaveParametersBtn.Location = new System.Drawing.Point(255, 241);
          this.SaveParametersBtn.Name = "SaveParametersBtn";
          this.SaveParametersBtn.Size = new System.Drawing.Size(196, 23);
          this.SaveParametersBtn.TabIndex = 26;
          this.SaveParametersBtn.Text = "Save Parameters";
          this.SaveParametersBtn.UseVisualStyleBackColor = true;
          this.SaveParametersBtn.Click += new System.EventHandler(this.SaveParametersBtn_Click);
          // 
          // CameraComboBox
          // 
          this.CameraComboBox.FormattingEnabled = true;
          this.CameraComboBox.Location = new System.Drawing.Point(255, 214);
          this.CameraComboBox.Name = "CameraComboBox";
          this.CameraComboBox.Size = new System.Drawing.Size(196, 21);
          this.CameraComboBox.TabIndex = 25;
          this.CameraComboBox.SelectedIndexChanged += new System.EventHandler(this.CameraComboBox_SelectedIndexChanged);
          // 
          // btnConfigureRegions
          // 
          this.btnConfigureRegions.Location = new System.Drawing.Point(255, 7);
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
          this.showPolygonsCheckbox.Location = new System.Drawing.Point(255, 36);
          this.showPolygonsCheckbox.Name = "showPolygonsCheckbox";
          this.showPolygonsCheckbox.Size = new System.Drawing.Size(98, 17);
          this.showPolygonsCheckbox.TabIndex = 23;
          this.showPolygonsCheckbox.Text = "Show polygons";
          this.showPolygonsCheckbox.UseVisualStyleBackColor = true;
          // 
          // label10
          // 
          this.label10.AutoSize = true;
          this.label10.Location = new System.Drawing.Point(352, 167);
          this.label10.Name = "label10";
          this.label10.Size = new System.Drawing.Size(82, 13);
          this.label10.TabIndex = 22;
          this.label10.Text = "Objects tracked";
          // 
          // trackCountBox
          // 
          this.trackCountBox.Location = new System.Drawing.Point(255, 164);
          this.trackCountBox.Name = "trackCountBox";
          this.trackCountBox.Size = new System.Drawing.Size(91, 20);
          this.trackCountBox.TabIndex = 21;
          this.trackCountBox.Text = "0";
          // 
          // pushStateCheckbox
          // 
          this.pushStateCheckbox.AutoSize = true;
          this.pushStateCheckbox.Location = new System.Drawing.Point(255, 190);
          this.pushStateCheckbox.Name = "pushStateCheckbox";
          this.pushStateCheckbox.Size = new System.Drawing.Size(117, 17);
          this.pushStateCheckbox.TabIndex = 20;
          this.pushStateCheckbox.Text = "Push state samples";
          this.pushStateCheckbox.UseVisualStyleBackColor = true;
          this.pushStateCheckbox.CheckedChanged += new System.EventHandler(this.pushStateCheckbox_CheckedChanged);
          // 
          // label9
          // 
          this.label9.AutoSize = true;
          this.label9.Location = new System.Drawing.Point(352, 89);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(99, 13);
          this.label9.TabIndex = 19;
          this.label9.Text = "Average noise area";
          // 
          // avgNoiseTextbox
          // 
          this.avgNoiseTextbox.Location = new System.Drawing.Point(255, 86);
          this.avgNoiseTextbox.Name = "avgNoiseTextbox";
          this.avgNoiseTextbox.Size = new System.Drawing.Size(91, 20);
          this.avgNoiseTextbox.TabIndex = 18;
          this.avgNoiseTextbox.Text = "0";
          // 
          // label8
          // 
          this.label8.AutoSize = true;
          this.label8.Location = new System.Drawing.Point(352, 62);
          this.label8.Name = "label8";
          this.label8.Size = new System.Drawing.Size(103, 13);
          this.label8.TabIndex = 17;
          this.label8.Text = "Average object area";
          // 
          // avgAreaTextbox
          // 
          this.avgAreaTextbox.Location = new System.Drawing.Point(255, 59);
          this.avgAreaTextbox.Name = "avgAreaTextbox";
          this.avgAreaTextbox.Size = new System.Drawing.Size(92, 20);
          this.avgAreaTextbox.TabIndex = 16;
          this.avgAreaTextbox.Text = "20000";
          // 
          // label6
          // 
          this.label6.AutoSize = true;
          this.label6.Location = new System.Drawing.Point(352, 141);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(84, 13);
          this.label6.TabIndex = 13;
          this.label6.Text = "Movement mass";
          // 
          // movementMassBox
          // 
          this.movementMassBox.Location = new System.Drawing.Point(255, 138);
          this.movementMassBox.Name = "movementMassBox";
          this.movementMassBox.Size = new System.Drawing.Size(91, 20);
          this.movementMassBox.TabIndex = 12;
          // 
          // label5
          // 
          this.label5.AutoSize = true;
          this.label5.Location = new System.Drawing.Point(353, 115);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(83, 13);
          this.label5.TabIndex = 11;
          this.label5.Text = "Detection count";
          // 
          // detectionCountBox
          // 
          this.detectionCountBox.Location = new System.Drawing.Point(255, 112);
          this.detectionCountBox.Name = "detectionCountBox";
          this.detectionCountBox.Size = new System.Drawing.Size(92, 20);
          this.detectionCountBox.TabIndex = 10;
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
          this.splitContainer2.Panel1.Controls.Add(this.imageBox2);
          this.splitContainer2.Panel1.Controls.Add(this.panel2);
          // 
          // splitContainer2.Panel2
          // 
          this.splitContainer2.Panel2.Controls.Add(this.imageBox3);
          this.splitContainer2.Panel2.Controls.Add(this.panel3);
          this.splitContainer2.Size = new System.Drawing.Size(530, 684);
          this.splitContainer2.SplitterDistance = 342;
          this.splitContainer2.TabIndex = 0;
          // 
          // imageBox2
          // 
          this.imageBox2.Dock = System.Windows.Forms.DockStyle.Fill;
          this.imageBox2.Location = new System.Drawing.Point(0, 20);
          this.imageBox2.Name = "imageBox2";
          this.imageBox2.Size = new System.Drawing.Size(530, 322);
          this.imageBox2.TabIndex = 10;
          this.imageBox2.TabStop = false;
          // 
          // panel2
          // 
          this.panel2.Controls.Add(this.label2);
          this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
          this.panel2.Location = new System.Drawing.Point(0, 0);
          this.panel2.Name = "panel2";
          this.panel2.Size = new System.Drawing.Size(530, 20);
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
          // imageBox3
          // 
          this.imageBox3.Dock = System.Windows.Forms.DockStyle.Fill;
          this.imageBox3.Location = new System.Drawing.Point(0, 20);
          this.imageBox3.Name = "imageBox3";
          this.imageBox3.Size = new System.Drawing.Size(530, 318);
          this.imageBox3.TabIndex = 6;
          this.imageBox3.TabStop = false;
          // 
          // panel3
          // 
          this.panel3.Controls.Add(this.label3);
          this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
          this.panel3.Location = new System.Drawing.Point(0, 0);
          this.panel3.Name = "panel3";
          this.panel3.Size = new System.Drawing.Size(530, 20);
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
          // TrafficCounter
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(1054, 687);
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
          ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
          this.panel2.ResumeLayout(false);
          this.panel2.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).EndInit();
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
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox detectionCountBox;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox movementMassBox;
      private Emgu.CV.UI.ImageBox imageBox3;
      private Emgu.CV.UI.ImageBox imageBox2;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.TextBox avgNoiseTextbox;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.TextBox avgAreaTextbox;
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
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.TextBox configPathBox;
      private System.Windows.Forms.TextBox debugTextBox;
   }
}