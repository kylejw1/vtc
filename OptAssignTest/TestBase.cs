using System.Drawing;
using VTC;
using VTC.Settings;

namespace OptAssignTest
{
    public class TestBase
    {
        /// <summary>
        /// Default vehicle size.
        /// </summary>
        protected const int VehicleRadius = 3; // in pixels

        /// <summary>
        /// Default settings.
        /// </summary>
        protected static ISettings DefaultSettings
        {
            get { return _defaultSettings; }
        }
        private static readonly ISettings _defaultSettings = CreateSettings(VehicleRadius);

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

            return new IntersectionVista(settings, (int) settings.FrameWidth, (int) settings.FrameHeight)
            {
                RegionConfiguration = regionConfig
            };
        }

        /// <summary>
        /// Assuming that vehicles is circle, create settings for it.
        /// </summary>
        /// <param name="vehicleRadius">Radius of "vehicle"</param>
        protected static ISettings CreateSettings(int vehicleRadius)
        {
            return new TestSettings { CarRadius = vehicleRadius };
        }
    }
}