using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;
using VTC.Kernel.Video;

namespace OptAssignTest
{
    [TestClass]
    public class CrossroadTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Two cars passing the same intersection. One goes vertically, the second one - horizontally.")]
        public void CrossingPaths() // TODO: cars should not go through the intersection at the same time
        {
            var script = CrossingPathScript();

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Both cars should be detected (failed at {0} frame).", frame);
                    // TODO: make sure that each car keeps its direction
                }
            });
        }

        private static Script CrossingPathScript()
        {
            var script = new Script();

            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(DefaultSettings);

            script
                .CreateCar(VehicleRadius)
                .AddHorizontalPath(DefaultSettings, Direction.East);

            return script;
        }

        {
            return new[]
            {
                new CaptureContext(new CaptureEmulator("Crossing paths", CrossingPathScript()), DefaultSettings)
            };
        }
    }
}
