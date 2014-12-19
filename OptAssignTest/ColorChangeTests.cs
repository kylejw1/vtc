using System;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptAssignTest
{
    [TestClass]
    public class ColorChangeTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Vehicle might (slightly?) change color, and it should not affect recognition")]
        public void ChangedCarColor_ShouldNotAffectTracking()
        {
            var settings = CreateSettings(VehicleRadius);

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(settings)
                .CarColor( // car changes color
                    delegate(uint frame)
                    {
                        switch (frame % 5)
                        {
                            case 0:
                            case 4:
                                return new Bgr(0xff, 0xff, 0xff);
                            case 1:
                            case 3:
                                return new Bgr(0xee, 0xee, 0xee);
                            case 2:
                                return new Bgr(0xdd, 0xdd, 0xdd);
                            default:
                                throw new Exception("what?");
                        }
                    });

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
