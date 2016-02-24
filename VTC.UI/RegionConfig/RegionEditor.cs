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

namespace VTC
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
            tlpPolygonToggles.Controls.Clear();

            if (null == RegionConfig) return;

            // Set table rows
            var rows = ((RegionConfig.Regions.Count() + 1) / 2) + 1;
            tlpPolygonToggles.RowCount = rows;

            PolygonLookup = new Dictionary<Button, Polygon>();

            var rb = BuildEditButton("ROI Mask");
            PolygonLookup[rb] = RegionConfig.RoiMask;
            tlpPolygonToggles.Controls.Add(rb);
            tlpPolygonToggles.SetColumnSpan(rb, 2);
            
            foreach (var regionKvp in RegionConfig.Regions)
            {
                rb = BuildEditButton(regionKvp.Key);
                PolygonLookup[rb] = regionKvp.Value;
                tlpPolygonToggles.Controls.Add(rb);
            }
        }

        private Button BuildEditButton(string text)
        {
            var tb = new Button();

            tb.Text = text;
            tb.Dock  = DockStyle.Fill;
            tb.AutoSize = true;
            tb.TextAlign = ContentAlignment.MiddleCenter;

            tb.MouseEnter += tb_MouseEnter;
            tb.MouseLeave += tb_MouseLeave;
            tb.Click += tb_Clicked;
            
            return tb;
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
                foreach (var button in PolygonLookup.Keys)
                {
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
    }
}
