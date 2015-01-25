using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;

namespace OptAssignTest
{
    [TestClass]
    public class TurnsTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Left turn should be detected correctly.")]
        public void LeftTurn()
        {
            var settings = CreateSettings(VehicleRadius);

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(settings, Direction.South, Direction.West);

            RunScript(settings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(1, vehicles.Count, "Car should be detected all the way.");
                }
            });
        }

        [TestMethod]
        [Description("")]
        public void RightAndStraight()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Description("")]
        public void DualTurns()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Description("")]
        public void DualTurnsWithLossDetection()
        {
            throw new NotImplementedException();
        }
    }
}
