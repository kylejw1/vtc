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
            this.timeActiveTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.serverUrlTextBox = new System.Windows.Forms.TextBox();
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
            this.MoGcheckBox = new System.Windows.Forms.CheckBox();
            this.delayProcessingCheckbox = new System.Windows.Forms.CheckBox();
            this.exportDatasetsCheckbox = new System.Windows.Forms.CheckBox();
            this.btnToggleVideoMux = new System.Windows.Forms.Button();
            this.watchdogTimer = new System.Windows.Forms.Timer(this.components);
            this.hideTrackersButton = new System.Windows.Forms.Button();
            this.disableOpticalFlowCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pushStateTimer
            // 
            this.pushStateTimer.Enabled = true;
            this.pushStateTimer.Interval = 10000;
            this.pushStateTimer.Tick += new System.EventHandler(this.PushStateProcess);
            // 
            // timeActiveTextBox
            // 
            this.timeActiveTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeActiveTextBox.Location = new System.Drawing.Point(12, 458);
            this.timeActiveTextBox.Name = "timeActiveTextBox";
            this.timeActiveTextBox.ReadOnly = true;
            this.timeActiveTextBox.Size = new System.Drawing.Size(100, 21);
            this.timeActiveTextBox.TabIndex = 65;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(118, 461);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 15);
            this.label11.TabIndex = 64;
            this.label11.Text = "Time active";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(358, 419);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(196, 22);
            this.button1.TabIndex = 63;
            this.button1.Text = "Resample background";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.resampleBackgroundButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(282, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 15);
            this.label7.TabIndex = 58;
            this.label7.Text = "Server URL";
            // 
            // serverUrlTextBox
            // 
            this.serverUrlTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverUrlTextBox.Location = new System.Drawing.Point(358, 209);
            this.serverUrlTextBox.Name = "serverUrlTextBox";
            this.serverUrlTextBox.Size = new System.Drawing.Size(195, 21);
            this.serverUrlTextBox.TabIndex = 57;
            // 
            // tbVistaStats
            // 
            this.tbVistaStats.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbVistaStats.Location = new System.Drawing.Point(14, 12);
            this.tbVistaStats.Multiline = true;
            this.tbVistaStats.Name = "tbVistaStats";
            this.tbVistaStats.ReadOnly = true;
            this.tbVistaStats.Size = new System.Drawing.Size(240, 429);
            this.tbVistaStats.TabIndex = 54;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(266, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 15);
            this.label4.TabIndex = 55;
            this.label4.Text = "Intersection ID";
            // 
            // intersectionIDTextBox
            // 
            this.intersectionIDTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intersectionIDTextBox.Location = new System.Drawing.Point(357, 183);
            this.intersectionIDTextBox.Name = "intersectionIDTextBox";
            this.intersectionIDTextBox.Size = new System.Drawing.Size(196, 21);
            this.intersectionIDTextBox.TabIndex = 53;
            // 
            // SaveParametersBtn
            // 
            this.SaveParametersBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveParametersBtn.Location = new System.Drawing.Point(358, 281);
            this.SaveParametersBtn.Name = "SaveParametersBtn";
            this.SaveParametersBtn.Size = new System.Drawing.Size(196, 22);
            this.SaveParametersBtn.TabIndex = 52;
            this.SaveParametersBtn.Text = "Save Configuration";
            this.SaveParametersBtn.UseVisualStyleBackColor = true;
            this.SaveParametersBtn.Click += new System.EventHandler(this.SaveParametersBtn_Click);
            // 
            // CameraComboBox
            // 
            this.CameraComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CameraComboBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CameraComboBox.FormattingEnabled = true;
            this.CameraComboBox.Location = new System.Drawing.Point(357, 12);
            this.CameraComboBox.Name = "CameraComboBox";
            this.CameraComboBox.Size = new System.Drawing.Size(196, 23);
            this.CameraComboBox.TabIndex = 51;
            this.CameraComboBox.SelectedIndexChanged += new System.EventHandler(this.CameraComboBox_SelectedIndexChanged);
            // 
            // btnConfigureRegions
            // 
            this.btnConfigureRegions.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfigureRegions.Location = new System.Drawing.Point(357, 122);
            this.btnConfigureRegions.Name = "btnConfigureRegions";
            this.btnConfigureRegions.Size = new System.Drawing.Size(196, 29);
            this.btnConfigureRegions.TabIndex = 50;
            this.btnConfigureRegions.Text = "Configure Regions";
            this.btnConfigureRegions.UseVisualStyleBackColor = true;
            this.btnConfigureRegions.Click += new System.EventHandler(this.btnConfigureRegions_Click);
            // 
            // showPolygonsCheckbox
            // 
            this.showPolygonsCheckbox.AutoSize = true;
            this.showPolygonsCheckbox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showPolygonsCheckbox.Location = new System.Drawing.Point(358, 97);
            this.showPolygonsCheckbox.Name = "showPolygonsCheckbox";
            this.showPolygonsCheckbox.Size = new System.Drawing.Size(110, 19);
            this.showPolygonsCheckbox.TabIndex = 49;
            this.showPolygonsCheckbox.Text = "Show polygons";
            this.showPolygonsCheckbox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(260, 160);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 15);
            this.label10.TabIndex = 48;
            this.label10.Text = "Objects tracked";
            // 
            // trackCountBox
            // 
            this.trackCountBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackCountBox.Location = new System.Drawing.Point(358, 157);
            this.trackCountBox.Name = "trackCountBox";
            this.trackCountBox.Size = new System.Drawing.Size(195, 21);
            this.trackCountBox.TabIndex = 47;
            // 
            // pushStateCheckbox
            // 
            this.pushStateCheckbox.AutoSize = true;
            this.pushStateCheckbox.Checked = true;
            this.pushStateCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pushStateCheckbox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pushStateCheckbox.Location = new System.Drawing.Point(358, 235);
            this.pushStateCheckbox.Name = "pushStateCheckbox";
            this.pushStateCheckbox.Size = new System.Drawing.Size(137, 19);
            this.pushStateCheckbox.TabIndex = 46;
            this.pushStateCheckbox.Text = "Push state samples";
            this.pushStateCheckbox.UseVisualStyleBackColor = true;
            this.pushStateCheckbox.CheckedChanged += new System.EventHandler(this.pushStateCheckbox_CheckedChanged);
            // 
            // MoGcheckBox
            // 
            this.MoGcheckBox.AutoSize = true;
            this.MoGcheckBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoGcheckBox.Location = new System.Drawing.Point(358, 470);
            this.MoGcheckBox.Name = "MoGcheckBox";
            this.MoGcheckBox.Size = new System.Drawing.Size(93, 19);
            this.MoGcheckBox.TabIndex = 70;
            this.MoGcheckBox.Text = "Enable MoG";
            this.MoGcheckBox.UseVisualStyleBackColor = true;
            this.MoGcheckBox.CheckedChanged += new System.EventHandler(this.MoGcheckBox_CheckedChanged);
            // 
            // delayProcessingCheckbox
            // 
            this.delayProcessingCheckbox.AutoSize = true;
            this.delayProcessingCheckbox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delayProcessingCheckbox.Location = new System.Drawing.Point(358, 493);
            this.delayProcessingCheckbox.Name = "delayProcessingCheckbox";
            this.delayProcessingCheckbox.Size = new System.Drawing.Size(122, 19);
            this.delayProcessingCheckbox.TabIndex = 71;
            this.delayProcessingCheckbox.Text = "Delay processing";
            this.delayProcessingCheckbox.UseVisualStyleBackColor = true;
            // 
            // exportDatasetsCheckbox
            // 
            this.exportDatasetsCheckbox.AutoSize = true;
            this.exportDatasetsCheckbox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportDatasetsCheckbox.Location = new System.Drawing.Point(358, 258);
            this.exportDatasetsCheckbox.Name = "exportDatasetsCheckbox";
            this.exportDatasetsCheckbox.Size = new System.Drawing.Size(176, 19);
            this.exportDatasetsCheckbox.TabIndex = 72;
            this.exportDatasetsCheckbox.Text = "Export datasets periodically";
            this.exportDatasetsCheckbox.UseVisualStyleBackColor = true;
            // 
            // btnToggleVideoMux
            // 
            this.btnToggleVideoMux.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToggleVideoMux.Location = new System.Drawing.Point(358, 363);
            this.btnToggleVideoMux.Name = "btnToggleVideoMux";
            this.btnToggleVideoMux.Size = new System.Drawing.Size(196, 22);
            this.btnToggleVideoMux.TabIndex = 63;
            this.btnToggleVideoMux.Text = "Toggle Video Mux";
            this.btnToggleVideoMux.UseVisualStyleBackColor = true;
            this.btnToggleVideoMux.Click += new System.EventHandler(this.btnToggleVideoMux_Click);
            // 
            // watchdogTimer
            // 
            this.watchdogTimer.Enabled = true;
            this.watchdogTimer.Interval = 5000;
            this.watchdogTimer.Tick += new System.EventHandler(this.watchdogTimer_Tick);
            // 
            // hideTrackersButton
            // 
            this.hideTrackersButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hideTrackersButton.Location = new System.Drawing.Point(358, 391);
            this.hideTrackersButton.Name = "hideTrackersButton";
            this.hideTrackersButton.Size = new System.Drawing.Size(196, 22);
            this.hideTrackersButton.TabIndex = 73;
            this.hideTrackersButton.Text = "Hide trackers";
            this.hideTrackersButton.UseVisualStyleBackColor = true;
            this.hideTrackersButton.Click += new System.EventHandler(this.hideTrackersButton_Click);
            // 
            // activateLicenseButton
            // 
            this.activateLicenseButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activateLicenseButton.Location = new System.Drawing.Point(12, 484);
            this.activateLicenseButton.Name = "activateLicenseButton";
            this.activateLicenseButton.Size = new System.Drawing.Size(175, 23);
            this.activateLicenseButton.TabIndex = 74;
            this.activateLicenseButton.Text = "Activate License";
            this.activateLicenseButton.UseVisualStyleBackColor = true;
            this.activateLicenseButton.Click += new System.EventHandler(this.activateLicenseButton_Click);
            // 
            // disableOpticalFlowCheckbox
            // 
            this.disableOpticalFlowCheckbox.AutoSize = true;
            this.disableOpticalFlowCheckbox.Checked = true;
            this.disableOpticalFlowCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disableOpticalFlowCheckbox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disableOpticalFlowCheckbox.Location = new System.Drawing.Point(358, 447);
            this.disableOpticalFlowCheckbox.Name = "disableOpticalFlowCheckbox";
            this.disableOpticalFlowCheckbox.Size = new System.Drawing.Size(139, 19);
            this.disableOpticalFlowCheckbox.TabIndex = 75;
            this.disableOpticalFlowCheckbox.Text = "Disable Optical Flow";
            this.disableOpticalFlowCheckbox.UseVisualStyleBackColor = true;
            // 
            // TrafficCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(569, 519);
            this.Controls.Add(this.disableOpticalFlowCheckbox);
            this.Controls.Add(this.hideTrackersButton);
            this.Controls.Add(this.exportDatasetsCheckbox);
            this.Controls.Add(this.delayProcessingCheckbox);
            this.Controls.Add(this.MoGcheckBox);
            this.Controls.Add(this.timeActiveTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnToggleVideoMux);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.serverUrlTextBox);
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
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(570, 428);
            this.Name = "TrafficCounter";
            this.Text = "VTC";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrafficCounter_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private Timer pushStateTimer;
      private TextBox timeActiveTextBox;
      private Label label11;
      private Button button1;
      private Label label7;
      private TextBox serverUrlTextBox;
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
      private CheckBox MoGcheckBox;
      private CheckBox delayProcessingCheckbox;
      private CheckBox exportDatasetsCheckbox;
	  private Button btnToggleVideoMux;
      private Timer watchdogTimer;
      private Button hideTrackersButton;
      private CheckBox disableOpticalFlowCheckbox;


   }
}