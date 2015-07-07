﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;
using VTC.Kernel.Video;

namespace OptAssignTest
{
    [TestClass]
    public class SizeChangeTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Vehicle increasing size while moving, and it should not affect recognition.")]
        public void CarSizeChange_ShouldNotAffectTracking()
        {
            var script = SizeChangeScript();

            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                }
            });
        }

        private static Script SizeChangeScript()
        {
            var script = new Script();

            // at the last frame car should be four times bigger
            var frameSizeDiff = 4.0 / DefaultSettings.FrameHeight;

            script
                .CreateCar()
                .AddVerticalPath(DefaultSettings)
                .SetSize(frame => (uint) (VehicleRadius * (1 + frame * frameSizeDiff)));
            return script;
        }

        public override IEnumerable<CaptureContext> GetCaptures()
        {
            return new[]
            {
                new CaptureContext(new CaptureEmulator("Car size change", SizeChangeScript()), DefaultSettings),
            };
        }
    }
}
