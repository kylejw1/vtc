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
    using Kernel.Video;
    using ThumbnailImage = Image<Emgu.CV.Structure.Bgr, float>;

    public partial class RegionEditor : Form
    {
        private Polygon EditingPolygon;
        private Dictionary<Button, Polygon> PolygonLookup = new Dictionary<Button, Polygon>();

        private PictureBox Preview = new PictureBox();

        private IEnumerable<ICaptureSource> _captureSources;
        private IRegionConfigDataAccessLayer _regionConfigDal;

        private Dictionary<string, ThumbnailImage> Thumbnails = new Dictionary<string, ThumbnailImage>();

        private BindingSource _regionConfigurationsBindingSource;


        public RegionEditor(IEnumerable<ICaptureSource> captureSources, IRegionConfigDataAccessLayer regionConfigDal)
        {
            InitializeComponent();

            _captureSources = captureSources;
            _regionConfigDal = regionConfigDal;

            var regionConfigs = _regionConfigDal.LoadRegionConfigList().ToList();

            _regionConfigurationsBindingSource = new BindingSource();
            _regionConfigurationsBindingSource.DataSource = regionConfigs;

            lbRegionConfigurations.DataSource = _regionConfigurationsBindingSource;
            lbRegionConfigurations.DisplayMember = "Title";
            lbRegionConfigurations.ValueMember = "";

            tbRegionConfigName.DataBindings.Add("Text", _regionConfigurationsBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

            foreach(var cs in captureSources)
            {
                var name = cs.Name;
                int i = 1;
                while(Thumbnails.Keys.Contains(name))
                {
                    name = string.Format("{0} ({1})", cs.Name, i++);
                }

                Thumbnails[name] = cs.QueryFrame().Convert<Bgr, float>();
            }
            var thumbnailBindingSource = new BindingSource();
            thumbnailBindingSource.DataSource = Thumbnails;
            cbCaptureSource.DataSource = thumbnailBindingSource;
            cbCaptureSource.DisplayMember = "Key";
            cbCaptureSource.ValueMember = "Value";

            Preview.Dock = DockStyle.Fill;
            panelImage.Controls.Add(Preview);


        }

        private RegionConfig SelectedRegionConfig
        {
            get
            {
                return lbRegionConfigurations.SelectedItem as RegionConfig;
            }
        }

        private ThumbnailImage _thumbnail;
        private ThumbnailImage Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                UpdateImage();
            }
        }
        private ThumbnailImage _mask;
        private ThumbnailImage Mask
        {
            get
            {
                return _mask;
            }
            set
            {
                _mask = value;
                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            if (null == Thumbnail)
            {
                Preview.Image = null;
                return;
            }

            if (null == Mask)
            {
                Preview.Image = Thumbnail.ToBitmap();
            } else
            {
                Preview.Image = Thumbnail.Add(Mask).ToBitmap();
            }
        }

        private void InitializeToggleButtons(RegionConfig regionConfig)
        {
            if (null == regionConfig)
                return;

            tlpPolygonToggles.RowStyles.Clear();
            tlpPolygonToggles.RowStyles.Add(new RowStyle() { SizeType=SizeType.AutoSize });
            tlpPolygonToggles.Controls.Clear();
            tlpPolygonToggles.RowCount = 1;

            var roiButton = CreateEditRegionButton("ROI", regionConfig.RoiMask);
            AddEditAndDeleteButtons(roiButton, null, regionConfig.RoiMask);

            foreach (var regionKvp in regionConfig.Regions)
            {
                var edit = CreateEditRegionButton(regionKvp.Key, regionKvp.Value);
                var delete = CreateDeleteButton(edit, regionKvp.Value);
                AddEditAndDeleteButtons(edit, delete, regionKvp.Value);
            }
        }

        private Button CreateEditRegionButton(string text, Polygon polygon)
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
                SetEditing(true, button, polygon);
            };

            return button;
        }

        private Button CreateDeleteButton(Button editButton, Polygon polygon)
        {
            var deleteButton = new Button();

            deleteButton.Font = new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            deleteButton.Size = new Size(22, 22);
            deleteButton.Text = "X";
            deleteButton.UseVisualStyleBackColor = false;

            deleteButton.Click += (sender, args) =>
            {
                if (DialogResult.Yes == MessageBox.Show("Remove region " + editButton.Text + "?", string.Empty, MessageBoxButtons.YesNo))
                {
                    foreach(var kvp in SelectedRegionConfig.Regions.Where(r => r.Value == polygon).ToList())
                    {
                        SelectedRegionConfig.Regions.Remove(kvp.Key);
                    }
                    
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
            Mask = null;
        }

        void tb_MouseEnter(object sender, EventArgs e)
        {
            var rb = sender as Button;

            var polygon = PolygonLookup[rb];
            var mask = polygon.GetMask(Thumbnail.Width, Thumbnail.Height, new Bgr(Color.Blue));

            Mask = mask;
        }

        private void SetEditing(bool editing, Button activeButton, Polygon polygon)
        {
            // Disable all buttons while editing
            tlpRegionConfigSelector.Enabled = !editing;
            btnOK.Enabled = !editing;
            btnCancel.Enabled = !editing;
            foreach (var control in tlpRegionConfigEditor.Controls)
            {
                if (control == panelImage || !(control is Control))
                    continue;

                ((Control)control).Enabled = !editing;
            }

            if (editing)
            {
                EditingPolygon = polygon;

                if (null != activeButton)
                {
                    var control = new PolygonBuilderControl(Thumbnail, PolygonLookup[activeButton]);
                    control.Dock = DockStyle.Fill;
                    control.OnDoneClicked += (sender, args) =>
                    {
                        EditingPolygon.Clear();
                        foreach (var coord in control.Coordinates)
                        {
                            EditingPolygon.Add(coord);
                        }

                        SetEditing(false, null, null);
                    };
                    control.OnCancelClicked += (sender, args) =>
                    {
                        SetEditing(false, null, null);
                    };
                    panelImage.Controls.Clear();
                    panelImage.Controls.Add(control);
                }
            }
            else
            {
                // Restore the preview image
                panelImage.Controls.Clear();
                panelImage.Controls.Add(Preview);
            }
        }

        private void btnAddApproachExit_Click(object sender, EventArgs e)
        {
            var selectedRegionConfig = lbRegionConfigurations.SelectedItem as RegionConfig;
            if (null == selectedRegionConfig)
                return;

            var input = new InputPrompt("Region Name", "Enter Region Name:");
            if (DialogResult.OK != input.ShowDialog())
                return;

            var polygon = new Polygon();
            var edit = CreateEditRegionButton(input.InputString, polygon);
            var delete = CreateDeleteButton(edit, polygon);
            AddEditAndDeleteButtons(edit, delete, polygon);

            selectedRegionConfig.Regions.Add(input.InputString, polygon);
        }

        object previousSelectedValue;
        private void lbRegionConfigurations_SelectedValueChanged(object sender, EventArgs e)
        {
            if (previousSelectedValue == lbRegionConfigurations.SelectedValue)
                return;

            if (null == lbRegionConfigurations.SelectedValue)
                return;

            previousSelectedValue = lbRegionConfigurations.SelectedValue;

            var selectedRegionConfig = lbRegionConfigurations.SelectedItem as RegionConfig;
            if (null == selectedRegionConfig)
                return;

            InitializeToggleButtons(selectedRegionConfig);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var input = new InputPrompt("Region Configuration Name", "Enter Region Configuration Name:");
            if (DialogResult.OK != input.ShowDialog())
                return;

            _regionConfigurationsBindingSource.Add(new RegionConfig() { Title = input.InputString });
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var item = lbRegionConfigurations.SelectedItem as RegionConfig;
            if (null == item)
                return;

            var result = MessageBox.Show("Are you sure you want to delete " + item.Title, "Confirm Delete", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                _regionConfigurationsBindingSource.Remove(item);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Discard changes?", "Confirm", MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
                return;

            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            var regionConfigs = _regionConfigurationsBindingSource.DataSource as List<RegionConfig>;
            if (null != regionConfigs)
            {
                _regionConfigDal.SaveRegionConfigList(regionConfigs);
            }

            Close();
        }

        private void cbCaptureSource_SelectedValueChanged(object sender, EventArgs e)
        {
            Thumbnail = cbCaptureSource.SelectedValue as Image<Bgr, float>;
        }
    }
}
