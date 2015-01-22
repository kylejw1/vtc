using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptAssignTest
{
    [TestClass]
    public class ColorChangeTests : ScriptedTestBase
    {
        private static readonly Bgr[] _colors = {
                                                    new Bgr(0xff, 0xff, 0xff),
                                                    new Bgr(0xee, 0xee, 0xee),
                                                    new Bgr(0xdd, 0xdd, 0xdd),
                                                    new Bgr(0xee, 0xee, 0xee),
                                                    new Bgr(0xff, 0xff, 0xff),
                                                };

        [TestMethod]
        [Description("Vehicle might (slightly?) change color, and it should not affect recognition")]
        public void OftenChangedCarColor_ShouldNotAffectTracking()
        {
            var settings = CreateSettings(VehicleRadius);

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                    .AddVerticalPath(settings)
                    .CarColor(frame => _colors[frame % _colors.Length]);// car slightly changes color *each* frame

            RunScript(settings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                }
            });
        }

        [TestMethod]
        [Description("Vehicle changes color along the path, and it should not affect recognition")]
        public void SlowlyChangedCarColor_ShouldNotAffectTracking()
        {
            var settings = CreateSettings(VehicleRadius);

            // car changes color on each segment
            var segmentLength = settings.VerticalPathLength() / _colors.Length;

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                    .AddVerticalPath(settings)
                    .CarColor(frame => _colors[frame / segmentLength]);

            RunScript(settings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                }
            });
        }
    }
}
