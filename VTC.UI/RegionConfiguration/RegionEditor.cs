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

        private Dictionary<Button, Polygon> PolygonLookup = new Dictionary<Button, Polygon>();
        private Button ActiveButton = null;

        private PictureBox Preview = new PictureBox();
        private PolygonBuilderControl PolygonBuilder = null;

        public RegionEditor(Image<Bgr, float> bgImage, RegionConfig regionConfig)
        {
            InitializeComponent();

            RegionConfig = regionConfig.DeepCopy();
            BgImage = bgImage;

            RegionConfig.Title = RegionConfig.Title ?? "Region Config";

            Preview.Size = BgImage.Size;
            Preview.Image = BgImage.ToBitmap();
            panelImage.Controls.Add(Preview);
            tbRegionConfigName.DataBindings.Add("Text", RegionConfig, "Title");

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
            AddEditAndDeleteButtons(roiButton, null, RegionConfig.RoiMask);

            foreach (var regionKvp in RegionConfig.Regions)
            {
                var edit = CreateEditRegionButton(regionKvp.Key);
                var delete = CreateDeleteButton(edit);
                AddEditAndDeleteButtons(edit, delete, regionKvp.Value);
            }
        }

        private Button CreateEditRegionButton(string text)
        {
            var button = new Button();

            button.Text = text;
            button.Dock = DockStyle.Fill;
            button.AutoSize = true;
            button.TextAlign = ContentAlignment.MiddleCenter;

            button.MouseEnter += tb_MouseEnter;
            button.MouseLeave += tb_MouseLeave;
            button.Click += (sender, args) =>
            {
                SetEditing(true, button);
            };

            return button;
        }

        private Button CreateDeleteButton(Button editButton)
        {
            var deleteButton = new Button();

            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.BackColor = SystemColors.ControlLight;
            deleteButton.ForeColor = Color.Red;
            deleteButton.Font = new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            deleteButton.Size = new Size(23, 23);
            deleteButton.Text = "X";
            deleteButton.UseVisualStyleBackColor = false;

            deleteButton.Click += (sender, args) =>
            {
                if (DialogResult.Yes == MessageBox.Show("Remove region " + editButton.Text + "?", string.Empty, MessageBoxButtons.YesNo))
                {
                    tlpPolygonToggles.Controls.Remove(deleteButton);
                    tlpPolygonToggles.Controls.Remove(editButton);
                    PolygonLookup.Remove(editButton);
                }
            };

            return deleteButton;
        }



        private void AddEditAndDeleteButtons(Button edit, Button delete, Polygon polygon)
        {
            if (null != polygon)
                PolygonLookup[edit] = polygon;

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

            SetEditing(true, ActiveButton);            
        }

        private void SetEditing(bool editing, Button activeButton)
        {
            if (editing)
            {
                tbRegionConfigName.ReadOnly = false;

                ActiveButton = activeButton;
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
                tbRegionConfigName.ReadOnly = true;
                ActiveButton = null;
                // Enable all buttons for future edits
                foreach (var ctrl in tlpPolygonToggles.Controls)
                {
                    var button = ctrl as Button;

                    if (null == button)
                        continue;

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
            var input = new InputPrompt("Region Name", "Enter Region Name:");
            if (DialogResult.OK != input.ShowDialog())
                return;

            var polygon = new Polygon();
            var edit = CreateEditRegionButton(input.InputString);
            var delete = CreateDeleteButton(edit);
            AddEditAndDeleteButtons(edit, delete, polygon);
        }
    }
}
