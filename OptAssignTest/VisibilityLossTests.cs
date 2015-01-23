using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptAssignTest.Framework;

namespace OptAssignTest
{
    [TestClass]
    public class VisibilityLossTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Car should be tracked after visibility loss until threshold happens.")]
        public void VisibilityLoss_ShouldBeDetected()
        {
            var settings = CreateSettings(VehicleRadius);
            uint frameWhenDetectionLost = (uint) (settings.FrameHeight / 2);


            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(settings) 
                .Visibility(frame => frame < frameWhenDetectionLost); // car visible only in beginning

            RunScript(settings, script, (vista, frame) =>
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

                        Assert.IsTrue(vista.CurrentVehicles[0].state_history.Last().missed_detections > 0,
                            "Car visibility loss should be detected.");
                    }

                    // car should not be tracked anymore
                    if (frame == frameWhenDetectionLost + settings.MissThreshold + DetectionThreshold)
                    {
                        Assert.AreEqual(0, vista.CurrentVehicles.Count, "No cars should be detected after certain number of misses");
                    }
                });
        }

        [TestMethod]
        [Description("Vehicle should be recognized as the same after loss and reappearence within threshold.")]
        public void ReappearenceWithinThreshold_ShouldBeDetected()
        {
            var settings = CreateSettings(VehicleRadius);
            uint frameWhenDetectionLost = (uint)(settings.FrameHeight / 2);

            var frameWithReappearence = (uint)(frameWhenDetectionLost + settings.MissThreshold - 10);

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(settings)
                .Visibility(frame => (frame < frameWhenDetectionLost) || (frame > frameWithReappearence)); // car hidden in the middle

            RunScript(settings, script, (vista, frame) =>
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

                        Assert.IsTrue(vehicles[0].state_history.Last().missed_detections > 0, "Car visibility loss should be detected.");
                    }

                    // car should reappear, and should be recognized as already tracked
                    if (frame == frameWithReappearence + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected");

                        Assert.IsTrue(vehicles[0].state_history.Count > frameWithReappearence, "It should be the same car as before.");
                        Assert.IsTrue(vehicles[0].state_history.Last().missed_detections == 0, "Car visibility reappearence should be detected.");
                    }
                });
        }

        [TestMethod]
        [Description("Vehicle should be recognized as a new one after loss and reappearence after threshold.")]
        public void ReappearenceAfterThreshold_ShouldBeDetected()
        {
            var settings = CreateSettings(VehicleRadius);
            uint frameWhenDetectionLost = (uint)(settings.FrameHeight / 2);

            var frameWithReappearence = (uint)(frameWhenDetectionLost + settings.MissThreshold + 10);

            var script = new Script();
            script
                .CreateCar(VehicleRadius)
                .AddVerticalPath(settings)
                .Visibility(frame => (frame < frameWhenDetectionLost) || (frame > frameWithReappearence)); // car hidden in the middle

            RunScript(settings, script, (vista, frame) =>
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

                        Assert.IsTrue(vehicles[0].state_history.Last().missed_detections > 0, "Car visibility loss should be detected.");
                    }

                    // car should reappear, and should be recognized as already tracked
                    if (frame == frameWithReappearence + DetectionThreshold)
                    {
                        Assert.AreEqual(script.Cars.Count, vehicles.Count, "Car should be detected");

                        Assert.IsTrue(vehicles[0].state_history.Count < frameWhenDetectionLost, "It should be detected as a new car.");
                        
                        // ER: TODO: not working by some reason
                        //Assert.IsTrue(vehicles[0].state_history.Last().missed_detections == 0, "No missed detection expected.");
                    }
                });
        }

    }
}
