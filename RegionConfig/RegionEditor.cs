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

namespace VTC
{
    public partial class RegionEditor : Form
    {
        private Image<Bgr, float> BackgroundImage { get; set; }
        private RegionConfig RegionConfig;

        private Dictionary<Button, Polygon> PolygonLookup;
        private Button ActiveButton = null;

        private PictureBox Preview = new PictureBox();
        private PolygonBuilderControl PolygonBuilder = null;

        public RegionEditor(Image<Bgr, float> bgImage, RegionConfig regionConfig)
        {
            InitializeComponent();

            RegionConfig = regionConfig;
            BackgroundImage = bgImage;

            Preview.Size = BackgroundImage.Size;
            Preview.Image = BackgroundImage.ToBitmap();
            panelImage.Controls.Add(Preview);

            InitializeToggleButtons();
        }

        public RegionConfig GetRegionConfig()
        {
            var rc = new RegionConfig(tlpPolygonToggles.RowCount - 1);

            for (int i = 0; i < PolygonLookup.Values.Count; i++)
            {
                var polygon = PolygonLookup.Values.ElementAt(i);

                if (i == 0)
                {
                    rc.RoiMask = polygon;
                    continue;
                }

                if (i % 2 == 1)
                {
                    rc.ApproachExits.ElementAt((i - 1) / 2).Approach = polygon;
                }
                else
                {
                    rc.ApproachExits.ElementAt((i - 1) / 2).Exit = polygon;
                }


            }

            return rc;
        }

        private void InitializeToggleButtons()
        {
            tlpPolygonToggles.Controls.Clear();

            if (null == RegionConfig) return;

            tlpPolygonToggles.RowCount = RegionConfig.ApproachExits.Count() + 1;

            PolygonLookup = new Dictionary<Button, Polygon>();

            var rb = BuildEditButton("ROI Mask");
            PolygonLookup[rb] = RegionConfig.RoiMask;
            tlpPolygonToggles.Controls.Add(rb);
            tlpPolygonToggles.SetColumnSpan(rb, 2);
            
            int i = 0;
            foreach (var approachExit in RegionConfig.ApproachExits)
            {
                rb = BuildEditButton("Approach " + (i+1));
                PolygonLookup[rb] = RegionConfig.ApproachExits[i].Approach;
                tlpPolygonToggles.Controls.Add(rb);
                
                rb = BuildEditButton("Exit " + (i + 1));
                PolygonLookup[rb] = RegionConfig.ApproachExits[i].Exit;
                tlpPolygonToggles.Controls.Add(rb);

                i++;
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
            tb.Click += tb_Clicked;
            
            return tb;
        }

        void tb_MouseEnter(object sender, EventArgs e)
        {
            var rb = sender as Button;

            var polygon = PolygonLookup[rb];
            var mask = polygon.GetMask(BackgroundImage.Width, BackgroundImage.Height, new Bgr(Color.Blue));

            Preview.Image = BackgroundImage.Add(mask).ToBitmap();
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
                    var control = new PolygonBuilderControl(BackgroundImage, PolygonLookup[activeButton]);
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
                Preview.Image = BackgroundImage.ToBitmap();
                panelImage.Controls.Clear();
                panelImage.Controls.Add(Preview);
            }
        }

        void control_OnDoneClicked(object sender, EventArgs e)
        {
            PolygonLookup[ActiveButton] = PolygonBuilder.Coordinates;
            
            SetEditing(false, null);
        }

        void control_OnCancelClicked(object sender, EventArgs e)
        {
            SetEditing(false, null);
        }
    }
}
