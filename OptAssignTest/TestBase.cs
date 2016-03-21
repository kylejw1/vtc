using System.Drawing;
using VTC.Common;
using VTC.Kernel.RegionConfig;
using VTC.Kernel.Vistas;

namespace OptAssignTest
{
    public class TestBase
    {
        /// <summary>
        /// Default vehicle size.
        /// </summary>
        protected const int VehicleRadius = 3; // in pixels

        public ISettings settings = new AppSettings();

        /// <summary>
        /// Creates initialized intersection vista to be used for tests.
        /// </summary>
        /// <returns></returns>
        protected static IntersectionVista CreateVista(ISettings settings)
        {
            // create mask for the whole image
            var polygon = new Polygon();
            polygon.AddRange(new[]
            {
                new Point(0, 0), 
                new Point(0, (int) settings.FrameHeight),
                new Point((int) settings.FrameWidth, (int) settings.FrameHeight), 
                new Point((int) settings.FrameWidth, 0),
                new Point(0, 0)
            });

            var regionConfig = new RegionConfig
            {
                RoiMask = polygon
            };

            return new IntersectionVista(settings, (int) settings.FrameWidth, (int) settings.FrameHeight, new RegionConfig())
            {
                RegionConfiguration = regionConfig
            };

        }
    }
}