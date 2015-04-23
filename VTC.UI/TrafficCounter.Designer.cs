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
            this.pushStateTimer = new System.Windows.Forms.Timer(this.components);
            this.updateSamplePoint = new System.Windows.Forms.Button();
            this.saveRGBSamplesCheckBox = new System.Windows.Forms.CheckBox();
            this.rgbCoordinateTextbox = new System.Windows.Forms.TextBox();
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
            this.SuspendLayout();
            // 
            // pushStateTimer
            // 
            this.pushStateTimer.Enabled = true;
            this.pushStateTimer.Interval = 10000;
            this.pushStateTimer.Tick += new System.EventHandler(this.PushStateProcess);
            // 
            // updateSamplePoint
            // 
            this.updateSamplePoint.Location = new System.Drawing.Point(120, 259);
            this.updateSamplePoint.Name = "updateSamplePoint";
            this.updateSamplePoint.Size = new System.Drawing.Size(75, 23);
            this.updateSamplePoint.TabIndex = 69;
            this.updateSamplePoint.Text = "Update";
            this.updateSamplePoint.UseVisualStyleBackColor = true;
            this.updateSamplePoint.Click += new System.EventHandler(this.updateSamplePoint_Click);
            // 
            // saveRGBSamplesCheckBox
            // 
            this.saveRGBSamplesCheckBox.AutoSize = true;
            this.saveRGBSamplesCheckBox.Location = new System.Drawing.Point(15, 288);
            this.saveRGBSamplesCheckBox.Name = "saveRGBSamplesCheckBox";
            this.saveRGBSamplesCheckBox.Size = new System.Drawing.Size(87, 17);
            this.saveRGBSamplesCheckBox.TabIndex = 68;
            this.saveRGBSamplesCheckBox.Text = "Sample RGB";
            this.saveRGBSamplesCheckBox.UseVisualStyleBackColor = true;
            // 
            // rgbCoordinateTextbox
            // 
            this.rgbCoordinateTextbox.Location = new System.Drawing.Point(11, 261);
            this.rgbCoordinateTextbox.Name = "rgbCoordinateTextbox";
            this.rgbCoordinateTextbox.Size = new System.Drawing.Size(100, 20);
            this.rgbCoordinateTextbox.TabIndex = 67;
            // 
            // exportTrainingImagesButton
            // 
            this.exportTrainingImagesButton.Location = new System.Drawing.Point(261, 125);
            this.exportTrainingImagesButton.Name = "exportTrainingImagesButton";
            this.exportTrainingImagesButton.Size = new System.Drawing.Size(196, 23);
            this.exportTrainingImagesButton.TabIndex = 66;
            this.exportTrainingImagesButton.Text = "Export training images";
            this.exportTrainingImagesButton.UseVisualStyleBackColor = true;
            this.exportTrainingImagesButton.Click += new System.EventHandler(this.exportTrainingImagesButton_Click);
            // 
            // timeActiveTextBox
            // 
            this.timeActiveTextBox.Location = new System.Drawing.Point(11, 326);
            this.timeActiveTextBox.Name = "timeActiveTextBox";
            this.timeActiveTextBox.ReadOnly = true;
            this.timeActiveTextBox.Size = new System.Drawing.Size(100, 20);
            this.timeActiveTextBox.TabIndex = 65;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(117, 329);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 64;
            this.label11.Text = "Time active";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(261, 323);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(196, 23);
            this.button1.TabIndex = 63;
            this.button1.Text = "Resample background";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.resampleBackgroundButton_Click);
            // 
            // frameHeightBox
            // 
            this.frameHeightBox.Location = new System.Drawing.Point(364, 39);
            this.frameHeightBox.Name = "frameHeightBox";
            this.frameHeightBox.Size = new System.Drawing.Size(37, 20);
            this.frameHeightBox.TabIndex = 62;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(407, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 61;
            this.label6.Text = "Height (px)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(303, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Width (px)";
            // 
            // frameWidthBox
            // 
            this.frameWidthBox.Location = new System.Drawing.Point(262, 39);
            this.frameWidthBox.Name = "frameWidthBox";
            this.frameWidthBox.Size = new System.Drawing.Size(37, 20);
            this.frameWidthBox.TabIndex = 59;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(463, 238);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 58;
            this.label7.Text = "Server URL";
            // 
            // serverUrlTextBox
            // 
            this.serverUrlTextBox.Location = new System.Drawing.Point(262, 235);
            this.serverUrlTextBox.Name = "serverUrlTextBox";
            this.serverUrlTextBox.Size = new System.Drawing.Size(195, 20);
            this.serverUrlTextBox.TabIndex = 57;
            // 
            // debugTextBox
            // 
            this.debugTextBox.Location = new System.Drawing.Point(14, 125);
            this.debugTextBox.Multiline = true;
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(240, 112);
            this.debugTextBox.TabIndex = 56;
            // 
            // tbVistaStats
            // 
            this.tbVistaStats.Location = new System.Drawing.Point(14, 23);
            this.tbVistaStats.Multiline = true;
            this.tbVistaStats.Name = "tbVistaStats";
            this.tbVistaStats.ReadOnly = true;
            this.tbVistaStats.Size = new System.Drawing.Size(240, 96);
            this.tbVistaStats.TabIndex = 54;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(463, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 55;
            this.label4.Text = "Intersection ID";
            // 
            // intersectionIDTextBox
            // 
            this.intersectionIDTextBox.Location = new System.Drawing.Point(261, 209);
            this.intersectionIDTextBox.Name = "intersectionIDTextBox";
            this.intersectionIDTextBox.Size = new System.Drawing.Size(196, 20);
            this.intersectionIDTextBox.TabIndex = 53;
            // 
            // SaveParametersBtn
            // 
            this.SaveParametersBtn.Location = new System.Drawing.Point(261, 294);
            this.SaveParametersBtn.Name = "SaveParametersBtn";
            this.SaveParametersBtn.Size = new System.Drawing.Size(196, 23);
            this.SaveParametersBtn.TabIndex = 52;
            this.SaveParametersBtn.Text = "Save Parameters";
            this.SaveParametersBtn.UseVisualStyleBackColor = true;
            this.SaveParametersBtn.Click += new System.EventHandler(this.SaveParametersBtn_Click);
            // 
            // CameraComboBox
            // 
            this.CameraComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CameraComboBox.FormattingEnabled = true;
            this.CameraComboBox.Location = new System.Drawing.Point(261, 12);
            this.CameraComboBox.Name = "CameraComboBox";
            this.CameraComboBox.Size = new System.Drawing.Size(196, 21);
            this.CameraComboBox.TabIndex = 51;
            this.CameraComboBox.SelectedIndexChanged += new System.EventHandler(this.CameraComboBox_SelectedIndexChanged);
            // 
            // btnConfigureRegions
            // 
            this.btnConfigureRegions.Location = new System.Drawing.Point(261, 94);
            this.btnConfigureRegions.Name = "btnConfigureRegions";
            this.btnConfigureRegions.Size = new System.Drawing.Size(196, 23);
            this.btnConfigureRegions.TabIndex = 50;
            this.btnConfigureRegions.Text = "Configure Regions";
            this.btnConfigureRegions.UseVisualStyleBackColor = true;
            this.btnConfigureRegions.Click += new System.EventHandler(this.btnConfigureRegions_Click);
            // 
            // showPolygonsCheckbox
            // 
            this.showPolygonsCheckbox.AutoSize = true;
            this.showPolygonsCheckbox.Location = new System.Drawing.Point(262, 71);
            this.showPolygonsCheckbox.Name = "showPolygonsCheckbox";
            this.showPolygonsCheckbox.Size = new System.Drawing.Size(98, 17);
            this.showPolygonsCheckbox.TabIndex = 49;
            this.showPolygonsCheckbox.Text = "Show polygons";
            this.showPolygonsCheckbox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(463, 186);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 48;
            this.label10.Text = "Objects tracked";
            // 
            // trackCountBox
            // 
            this.trackCountBox.Location = new System.Drawing.Point(262, 183);
            this.trackCountBox.Name = "trackCountBox";
            this.trackCountBox.Size = new System.Drawing.Size(195, 20);
            this.trackCountBox.TabIndex = 47;
            // 
            // pushStateCheckbox
            // 
            this.pushStateCheckbox.AutoSize = true;
            this.pushStateCheckbox.Location = new System.Drawing.Point(262, 261);
            this.pushStateCheckbox.Name = "pushStateCheckbox";
            this.pushStateCheckbox.Size = new System.Drawing.Size(117, 17);
            this.pushStateCheckbox.TabIndex = 46;
            this.pushStateCheckbox.Text = "Push state samples";
            this.pushStateCheckbox.UseVisualStyleBackColor = true;
            this.pushStateCheckbox.CheckedChanged += new System.EventHandler(this.pushStateCheckbox_CheckedChanged);
            // 
            // TrafficCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 361);
            this.Controls.Add(this.updateSamplePoint);
            this.Controls.Add(this.saveRGBSamplesCheckBox);
            this.Controls.Add(this.rgbCoordinateTextbox);
            this.Controls.Add(this.exportTrainingImagesButton);
            this.Controls.Add(this.timeActiveTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.frameHeightBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.frameWidthBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.serverUrlTextBox);
            this.Controls.Add(this.debugTextBox);
            this.Controls.Add(this.tbVistaStats);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.intersectionIDTextBox);
            this.Controls.Add(this.SaveParametersBtn);
            this.Controls.Add(this.CameraComboBox);
            this.Controls.Add(this.btnConfigureRegions);
            this.Controls.Add(this.showPolygonsCheckbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.trackCountBox);
            this.Controls.Add(this.pushStateCheckbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrafficCounter";
            this.Text = "VTC";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrafficCounter_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private Timer pushStateTimer;
      private Button updateSamplePoint;
      private CheckBox saveRGBSamplesCheckBox;
      private TextBox rgbCoordinateTextbox;
      private Button exportTrainingImagesButton;
      private TextBox timeActiveTextBox;
      private Label label11;
      private Button button1;
      private TextBox frameHeightBox;
      private Label label6;
      private Label label5;
      private TextBox frameWidthBox;
      private Label label7;
      private TextBox serverUrlTextBox;
      private TextBox debugTextBox;
      private TextBox tbVistaStats;
      private Label label4;
      private TextBox intersectionIDTextBox;
      private Button SaveParametersBtn;
      private ComboBox CameraComboBox;
      private Button btnConfigureRegions;
      private CheckBox showPolygonsCheckbox;
      private Label label10;
      private TextBox trackCountBox;
      private CheckBox pushStateCheckbox;


   }
}