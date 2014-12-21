using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptAssignTest
{
    [TestClass]
    public class CrossroadTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Two cars passing the same intersection. One goes vertically, the second one - horizontally.")]
        public void CrossingPaths() // TODO: cars should not go through the intersection at the same time
        {
            var settings = CreateSettings(VehicleRadius);

            var script = new Script();

            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(settings);

            script
                .CreateCar(VehicleRadius)
                .AddHorizontalPath(settings);

            RunScript(settings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Both cars should be detected.");
                }
            });
        }
    }
}
