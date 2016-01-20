namespace LicenseManager
{
    partial class LicenseManagerView
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
            this.btnLater = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbKey = new System.Windows.Forms.TextBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblKeyError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLater
            // 
            this.btnLater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLater.Location = new System.Drawing.Point(289, 197);
            this.btnLater.Name = "btnLater";
            this.btnLater.Size = new System.Drawing.Size(107, 23);
            this.btnLater.TabIndex = 0;
            this.btnLater.Text = "Later";
            this.btnLater.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(338, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter your product key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(33, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(282, 40);
            this.label2.TabIndex = 2;
            this.label2.Text = "Your product key is 28 characters.\r\nPlease enter it now and press Activate.";
            // 
            // tbKey
            // 
            this.tbKey.Location = new System.Drawing.Point(78, 136);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(282, 20);
            this.tbKey.TabIndex = 3;
            this.tbKey.TextChanged += new System.EventHandler(this.tbKey_TextChanged);
            this.tbKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbKey_KeyPress);
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(176, 197);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(107, 23);
            this.btnActivate.TabIndex = 0;
            this.btnActivate.Text = "Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(33, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Key:";
            // 
            // lblKeyError
            // 
            this.lblKeyError.AutoSize = true;
            this.lblKeyError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyError.ForeColor = System.Drawing.Color.Red;
            this.lblKeyError.Location = new System.Drawing.Point(78, 163);
            this.lblKeyError.Name = "lblKeyError";
            this.lblKeyError.Size = new System.Drawing.Size(0, 13);
            this.lblKeyError.TabIndex = 4;
            // 
            // LicenseManagerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 232);
            this.Controls.Add(this.lblKeyError);
            this.Controls.Add(this.tbKey);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.btnLater);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseManagerView";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Activation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLater;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbKey;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblKeyError;
    }
}

