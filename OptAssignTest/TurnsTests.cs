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
            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.South, Direction.West);

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(1, vehicles.Count, "Car should be detected all the way.");
                }
            });
        }

        [TestMethod]
        [Description("One car goes straight thru intersection, another turns right.")]
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
