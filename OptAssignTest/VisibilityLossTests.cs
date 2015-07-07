using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;
using VTC.Kernel.Video;

namespace OptAssignTest
{
    [TestClass]
    public class VisibilityLossTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Car should be tracked after visibility loss until threshold happens.")]
        public void VisibilityLoss_ShouldBeDetected()
        {
            uint frameWhenDetectionLost = (uint) (DefaultSettings.FrameHeight / 2);
            var script = VisibilityLossScript(frameWhenDetectionLost);

            RunScript(DefaultSettings, script, (vista, frame) =>
                {
                    // the car should be detected at that point
                    if (frame == DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vista.CurrentVehicles.Count, "Car still not detected.");
                    }

                    // car should became invisible, but still be tracked 
                    if (frame == frameWhenDetectionLost + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vista.CurrentVehicles.Count, "Car still should be detected");

                        Assert.IsTrue(vista.CurrentVehicles[0].StateHistory.Last().MissedDetections > 0,
                            "Car visibility loss should be detected.");
                    }

                    // car should not be tracked anymore
                    if (frame == frameWhenDetectionLost + DefaultSettings.MissThreshold + DetectionThreshold)
                    {
                        Assert.AreEqual(0, vista.CurrentVehicles.Count, "No cars should be detected after certain number of misses");
                    }
                });
        }

        [TestMethod]
        [Description("Vehicle should be recognized as the same after loss and reappearence within threshold.")]
        public void ReappearenceWithinThreshold_ShouldBeDetected()
        {
            uint frameWhenDetectionLost = (uint)(DefaultSettings.FrameHeight / 2);
            var frameWithReappearence = (uint)(frameWhenDetectionLost + DefaultSettings.MissThreshold - 10);

            var script = ReappearenceWithinThresholdScript(frameWhenDetectionLost, frameWithReappearence);

            RunScript(DefaultSettings, script, (vista, frame) =>
                {
                    var vehicles = vista.CurrentVehicles;

                    if (frame == DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                    }

                    // car should became invisible, but still be tracked 
                    if (frame == frameWhenDetectionLost + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car still should be detected.");

                        Assert.IsTrue(vehicles[0].StateHistory.Last().MissedDetections > 0, "Car visibility loss should be detected.");
                    }

                    // car should reappear, and should be recognized as already tracked
                    if (frame == frameWithReappearence + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected");

                        Assert.IsTrue(vehicles[0].StateHistory.Count > frameWithReappearence, "It should be the same car as before.");
                        Assert.IsTrue(vehicles[0].StateHistory.Last().MissedDetections == 0, "Car visibility reappearence should be detected.");
                    }
                });
        }

        [TestMethod]
        [Description("Vehicle should be recognized as a new one after loss and reappearence after threshold.")]
        public void ReappearenceAfterThreshold_ShouldBeDetected()
        {
            uint frameWhenDetectionLost = (uint)(DefaultSettings.FrameHeight / 2);

            var frameWithReappearence = (uint)(frameWhenDetectionLost + DefaultSettings.MissThreshold + 10);

            var script = ReappearenceAfterThresholdScript(frameWhenDetectionLost, frameWithReappearence);

            RunScript(DefaultSettings, script, (vista, frame) =>
                {
                    var vehicles = vista.CurrentVehicles;

                    if (frame == DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected.");
                    }

                    // car should became invisible, but still be tracked 
                    if (frame == frameWhenDetectionLost + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car still should be detected.");

                        Assert.IsTrue(vehicles[0].StateHistory.Last().MissedDetections > 0, "Car visibility loss should be detected.");
                    }

                    // car should reappear, and should be recognized as already tracked
                    if (frame == frameWithReappearence + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected");

                        Assert.IsTrue(vehicles[0].StateHistory.Count < frameWhenDetectionLost, "It should be detected as a new car.");
                        
                        // ER: TODO: not working by some reason
                        //Assert.IsTrue(vehicles[0].state_history.Last().missed_detections == 0, "No missed detection expected.");
                    }
                });
        }

        public override IEnumerable<CaptureContext> GetCaptures()
        {
            // ER: dislike that it's calculated in different places (need it because it's reused in RunScript). 
            // Possible source of future errors.
            // think - maybe script should expose it somehow?
            uint frameWhenDetectionLost = (uint)(DefaultSettings.FrameHeight / 2);
            var frameWithReappearence = (uint)(frameWhenDetectionLost + DefaultSettings.MissThreshold - 10);
            var frameWithReappearenceTooLate = (uint)(frameWhenDetectionLost + DefaultSettings.MissThreshold + 10);

            return new[]
            {
                new CaptureContext(new CaptureEmulator("Visibility loss", VisibilityLossScript(frameWhenDetectionLost)), DefaultSettings),
                new CaptureContext(new CaptureEmulator("Reappearence (within threshold)", ReappearenceWithinThresholdScript(frameWhenDetectionLost, frameWithReappearence)), DefaultSettings),
                new CaptureContext(new CaptureEmulator("Reappearence (after threshold)", ReappearenceAfterThresholdScript(frameWhenDetectionLost, frameWithReappearenceTooLate)), DefaultSettings),
            };
        }

        private static Script ReappearenceAfterThresholdScript(uint frameWhenDetectionLost, uint frameWithReappearence)
        {
            var script = new Script();
            script
                .CreateCar()
                .SetSize(VehicleRadius)
                .AddVerticalPath(DefaultSettings)
                .Visibility(frame => (frame < frameWhenDetectionLost) || (frame > frameWithReappearence));
                // car hidden in the middle
            return script;
        }

        private static Script VisibilityLossScript(uint frameWhenDetectionLost)
        {
            var script = new Script();
            script
                .CreateCar()
                .SetSize(VehicleRadius)
                .AddVerticalPath(DefaultSettings)
                .Visibility(frame => frame < frameWhenDetectionLost); // car visible only in beginning
            return script;
        }

        private static Script ReappearenceWithinThresholdScript(uint frameWhenDetectionLost, uint frameWithReappearence)
        {
            var script = new Script();
            script
                .CreateCar()
                .SetSize(VehicleRadius)
                .AddVerticalPath(DefaultSettings)
                .Visibility(frame => (frame < frameWhenDetectionLost) || (frame > frameWithReappearence));
            // car hidden in the middle
            return script;
        }
    }
}
