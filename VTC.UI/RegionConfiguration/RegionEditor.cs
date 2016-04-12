using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    public partial class RegionEditor : Form
    {
        private Image<Bgr, float> BgImage { get; set; }
        public RegionConfig RegionConfig { get; private set; }

        private Dictionary<Button, Polygon> PolygonLookup;
        private Button ActiveButton = null;

        private PictureBox Preview = new PictureBox();
        private PolygonBuilderControl PolygonBuilder = null;

        public RegionEditor(Image<Bgr, float> bgImage, RegionConfig regionConfig)
        {
            InitializeComponent();

            RegionConfig = regionConfig.DeepCopy();
            BgImage = bgImage;

            Preview.Size = BgImage.Size;
            Preview.Image = BgImage.ToBitmap();
            panelImage.Controls.Add(Preview);
            

            InitializeToggleButtons();
        }

        private void InitializeToggleButtons()
        {
            if (null == RegionConfig)
                return;

            tlpPolygonToggles.RowStyles.Clear();
            tlpPolygonToggles.RowStyles.Add(new RowStyle() { SizeType=SizeType.AutoSize });
            tlpPolygonToggles.Controls.Clear();
            tlpPolygonToggles.RowCount = 1;

            var roiButton = CreateEditRegionButton("ROI");
            AddEditAndDeleteButtons(roiButton, null);

            foreach (var regionKvp in RegionConfig.Regions)
            {
                var edit = CreateEditRegionButton(regionKvp.Key);
                var delete = CreateDeleteButton(regionKvp.Value);
                AddEditAndDeleteButtons(edit, delete);
            }
        }

        private Button CreateEditRegionButton(string text)
        {
            var button = new Button();

            button.Text = text;
            button.Dock = DockStyle.Fill;
            button.AutoSize = true;
            button.TextAlign = ContentAlignment.MiddleCenter;

     //       button.MouseEnter += tb_MouseEnter;
     //       button.MouseLeave += tb_MouseLeave;
     //       button.Click += tb_Clicked;

            return button;
        }

        private Button CreateDeleteButton(Polygon polygon)
        {
            var deleteButton = new Button();

          //  deleteButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      //      deleteButton.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
        //    deleteButton.Size = new System.Drawing.Size(23, 23);
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.ForeColor = Color.Red;
            deleteButton.TabIndex = 6;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = false;

            deleteButton.BackColor = System.Drawing.SystemColors.ControlLight;

            deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            deleteButton.Text = "X";
            deleteButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            deleteButton.Click += (sender, args) =>
            {

            };

            return deleteButton;
        }



        private void AddEditAndDeleteButtons(Button edit, Button delete)
        {
            tlpPolygonToggles.Controls.Remove(btnAddApproachExit);
            if (null != edit)
                tlpPolygonToggles.Controls.Add(edit, 0, tlpPolygonToggles.RowCount - 1);
            if (null != delete) 
                tlpPolygonToggles.Controls.Add(delete, 1, tlpPolygonToggles.RowCount - 1);

            tlpPolygonToggles.RowCount++;
            tlpPolygonToggles.Controls.Add(btnAddApproachExit, 0, tlpPolygonToggles.RowCount - 1);
        }

        void tb_MouseLeave(object sender, EventArgs e)
        {
            Preview.Image = BgImage.ToBitmap();
        }

        void tb_MouseEnter(object sender, EventArgs e)
        {
            var rb = sender as Button;

            var polygon = PolygonLookup[rb];
            var mask = polygon.GetMask(BgImage.Width, BgImage.Height, new Bgr(Color.Blue));

            Preview.Image = BgImage.Add(mask).ToBitmap();
        }

        void tb_Clicked(object sender, EventArgs e)
        {
            var b = sender as Button;

            ActiveButton = b;

            SetEditing(true, ActiveButton);            
        }

        private void SetEditing(bool editing, Button activeButton)
        {
            if (editing)
            {
                // Disable all buttons while editing
                foreach (var ctrl in tlpPolygonToggles.Controls)
                {
                    var button = ctrl as Button;

                    if (null == button)
                        continue;

                    button.Enabled = false;
                }

                btnOK.Enabled = false;
                btnCancel.Enabled = false;

                if (null != activeButton)
                {
                    var control = new PolygonBuilderControl(BgImage, PolygonLookup[activeButton]);
                    control.Dock = DockStyle.Fill;
                    control.OnCancelClicked += control_OnCancelClicked;
                    control.OnDoneClicked += control_OnDoneClicked;
                    PolygonBuilder = control;
                    panelImage.Controls.Clear();
                    panelImage.Controls.Add(control);
                }
            }
            else
            {
                // Enable all buttons for future edits
                foreach (var button in PolygonLookup.Keys)
                {
                    button.Enabled = true;
                }

                btnOK.Enabled = true;
                btnCancel.Enabled = true;

                // Restore the preview image
                PolygonBuilder = null;
                Preview.Image = BgImage.ToBitmap();
                panelImage.Controls.Clear();
                panelImage.Controls.Add(Preview);
            }
        }

        void control_OnDoneClicked(object sender, EventArgs e)
        {
            PolygonLookup[ActiveButton].Clear();
            foreach (var coord in PolygonBuilder.Coordinates) {
                PolygonLookup[ActiveButton].Add(coord);
            }
            
            SetEditing(false, null);
        }

        void control_OnCancelClicked(object sender, EventArgs e)
        {
            SetEditing(false, null);
        }

        private void btnAddApproachExit_Click(object sender, EventArgs e)
        {
            var polygon = new Polygon();
            var edit = CreateEditRegionButton("new region");
            var delete = CreateDeleteButton(polygon);
            AddEditAndDeleteButtons(edit, delete);
        }
    }
}
