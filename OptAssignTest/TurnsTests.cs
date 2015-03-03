﻿using System;
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
            var script = new Script();

            // vehicle enters at bottom and turns right at center
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.South, Direction.East, new Path.Vector(VehicleRadius + 1, 0));

            // vehicle goes from bottom to up
            script
                .CreateCar(VehicleRadius)
                    .AddVerticalPath(DefaultSettings, Direction.South, new Path.Vector(-VehicleRadius, 0));

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(2, vehicles.Count, "Cars should be detected all the way.");

                    // TODO: validate that exit points are correct
                }
            });
        }

        [TestMethod]
        [Description("Two simultaneous turns should be detected correctly.")]
        public void DualTurns()
        {
            var script = new Script();

            // vehicle enters at bottom and turns left at center
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.South, Direction.West, new Path.Vector(0, -VehicleRadius));

            // vehicle enters at top and turns right at center
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.North, Direction.East, new Path.Vector(0, VehicleRadius + 1));

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(2, vehicles.Count, "Both cars should be tracked all the way.");

                    // TODO: validate that exit points are correct
                }
            });
        }

        [TestMethod]
        [Description("Two simultaneous turns should be detected correctly, even if one of the vehicles loss detection.")]
        public void DualTurnsWithLossDetection()
        {
            var script = new Script();

            var expectedFrames = (uint) (DefaultSettings.FrameWidth + DefaultSettings.FrameHeight);
            var expectedTurnFrame = expectedFrames/2;

            // vehicle enters at bottom and turns left at center
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.South, Direction.West, new Path.Vector(0, -VehicleRadius));

            // vehicle enters at top and turns right at center
            script
                .CreateCar(VehicleRadius)
                    .AddTurn(DefaultSettings, Direction.North, Direction.East, new Path.Vector(0, VehicleRadius + 1))
                    .Visibility(frame => Math.Abs(expectedTurnFrame - frame) > 5); // loss of detection during turn

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(2, vehicles.Count, "Both cars should be tracked all the way.");

                    // TODO: validate that exit points are correct
                }
            });
        }

        [TestMethod]
        [Ignore]
        [Description("Two simultaneous turns should be detected correctly, even if both of the vehicles loss detection. Vehicles are in different colors.")]
        public void DualTurnsWithDualLossDetection()
        {
            throw new NotImplementedException();
        }
    }
}