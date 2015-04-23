﻿using System.Collections.Generic;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;
using VTC.Kernel.Video;

namespace OptAssignTest
{
    [TestClass]
    public class ColorChangeTests : ScriptedTestBase
    {
        private static readonly Bgr[] _colors = {
                                                    new Bgr(0xff, 0xff, 0xff),
                                                    new Bgr(0xbb, 0xbb, 0xbb),
                                                    new Bgr(0x88, 0x88, 0x88),
                                                    new Bgr(0xbb, 0xbb, 0xbb),
                                                    new Bgr(0xff, 0xff, 0xff),
                                                };

        [TestMethod]
        [Description("Vehicle might (slightly?) change color, and it should not affect recognition")]
        public void OftenChangedCarColor_ShouldNotAffectTracking()
        {
            var script = OftenColorChangeScript();
            RunScript(DefaultSettings, script, (vista, frame) =>
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
            var script = SlowlyChangedColorScript();
            RunScript(DefaultSettings, script, (vista, frame) =>
            {
                var vehicles = vista.CurrentVehicles;

                if (frame > DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                }
            });
        }

        public override IEnumerable<CaptureContext> GetCaptures()
        {
            return new[]
            {
                new CaptureContext(new CaptureEmulator("Often color change", OftenColorChangeScript()), DefaultSettings),
                new CaptureContext(new CaptureEmulator("Slowly color change", SlowlyChangedColorScript()), DefaultSettings)
            };
        }

        private static Script OftenColorChangeScript()
        {
            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(DefaultSettings)
                .CarColor(frame => _colors[frame%_colors.Length]); // car slightly changes color *each* frame
            return script;
        }

        private static Script SlowlyChangedColorScript()
        {
            // car changes color on each segment
            var segmentLength = DefaultSettings.VerticalPathLength()/_colors.Length;

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(DefaultSettings)
                .CarColor(frame => _colors[frame/segmentLength]);
            return script;
        }
    }
}
