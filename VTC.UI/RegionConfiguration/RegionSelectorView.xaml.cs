using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VTC.Kernel.RegionConfig;

namespace VTC.RegionConfiguration
{
    /// <summary>
    /// Interaction logic for RegionSelectorView.xaml
    /// </summary>
    public partial class RegionSelectorView : UserControl
    {
        public RegionSelectorView(IEnumerable<CaptureSource.CaptureSource> captureSources, IEnumerable<RegionConfig> regionConfigs)
        {
            InitializeComponent();

            List<RegionSelectorItem> items = new List<RegionSelectorItem>();

            foreach (var captureSource in captureSources)
            {
                var thumb = captureSource.QueryFrame().ToBitmap();
                var thumbSource = BitmapToBitmapSource(thumb);

                var title = captureSource.Name;

                items.Add(new RegionSelectorItem()
                {
                    Thumbnail=thumbSource,
                    Title=title,
                    RegionConfigs = regionConfigs
                });
            }

            if (null != lbTodoList)
                lbTodoList.Items.Clear();
            lbTodoList.ItemsSource = items;
        }

        private BitmapSource BitmapToBitmapSource(Bitmap bmp)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               bmp.GetHbitmap(),
               IntPtr.Zero,
               System.Windows.Int32Rect.Empty,
               BitmapSizeOptions.FromWidthAndHeight(150, 100));
        }

        private void lbTodoList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }

        private void btnShowSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lbTodoList.SelectedItems)
                MessageBox.Show((o as RegionSelectorItem).Title);
        }

        private void btnSelectLast_Click(object sender, RoutedEventArgs e)
        {
            lbTodoList.SelectedIndex = lbTodoList.Items.Count - 1;
        }

        private void btnSelectNext_Click(object sender, RoutedEventArgs e)
        {
            int nextIndex = 0;
            if ((lbTodoList.SelectedIndex >= 0) && (lbTodoList.SelectedIndex < (lbTodoList.Items.Count - 1)))
                nextIndex = lbTodoList.SelectedIndex + 1;
            lbTodoList.SelectedIndex = nextIndex;
        }

        private void btnSelectCSharp_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lbTodoList.Items)
            {
                if ((o is RegionSelectorItem) && ((o as RegionSelectorItem).Title.Contains("C#")))
                {
                    lbTodoList.SelectedItem = o;
                    break;
                }
            }
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lbTodoList.Items)
                lbTodoList.SelectedItems.Add(o);
        }
    }
    public class RegionSelectorItem
    {
        public string Title { get; set; }
        public ImageSource Thumbnail { get; set; }
        public IEnumerable<RegionConfig> RegionConfigs { get; set; }
    }
}
