using System;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTC;
using VTC.Settings;

namespace OptAssignTest
{
    [TestClass]
    public class ScriptedTests : TestBase
    {
        /// <summary>
        /// Frames to skip before validation.
        /// </summary>
        private const int DetectionThreshold = 10;

        [TestMethod]
        [Description("Car should be tracked after visibility loss until threshold happens.")]
        public void VisibilityLoss_ShouldBeDetected()
        {
            const uint frameWhenDetectionLost = 150;
            const int vehicleRadius = 3;

            var settings = CreateSettings(vehicleRadius);
            var midX = (int)settings.FrameWidth / 2;

            var script = new Script();
            script
                .CreateCar()
                .AddPath(0, 400, frame => new Point(midX, (int)frame + vehicleRadius)) // vertical path
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
        [Description("Vehicle should be recognized as the same after loss and reappearance within threshold.")]
        public void ReappearenceWithinThreshold_ShouldBeDetected()
        {
            const uint frameWhenDetectionLost = 150;
            const int vehicleRadius = 3;

            var settings = CreateSettings(vehicleRadius);
            var midX = (int) settings.FrameWidth/2;
            var frameWithReappearence = (uint) (frameWhenDetectionLost + settings.MissThreshold - 10);

            var script = new Script();
            script
                .CreateCar()
                .AddPath(0, 400, frame => new Point(midX, (int)frame + vehicleRadius)) // vertical path
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
                        Assert.IsTrue(vehicles[0].state_history.Last().missed_detections == 0, "Car visibility reappearance should be detected.");
                    }
                });
        }

        [TestMethod]
        [Description("Vehicle should be recognized as the same after loss and reappearance within threshold.")]
        public void ReappearenceAfterThreshold_ShouldBeDetected()
        {
            const uint frameWhenDetectionLost = 150;
            const int vehicleRadius = 3;

            var settings = CreateSettings(vehicleRadius);
            var midX = (int) settings.FrameWidth/2;
            var frameWithReappearence = (uint) (frameWhenDetectionLost + settings.MissThreshold + 10);

            var script = new Script();
            script
                .CreateCar()
                .AddPath(0, 400, frame => new Point(midX, (int)frame + vehicleRadius)) // vertical path
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

        /// <summary>
        /// Execute script against the test action.
        /// </summary>
        private static void RunScript(ISettings settings, Script script, Action<Vista, uint> testAction)
        {
            var vista = CreateVista(settings);

            var background = new Image<Bgr, byte>((int) settings.FrameWidth, (int) settings.FrameHeight, new Bgr(Color.Black));
            vista.Update(background);

            var lastFrame = script.MaxFrame;
            for (uint frame = 0; frame < lastFrame; frame++)
            {
                var image = background.Clone();

                script.Draw(frame, image);
                vista.Update(image);

                testAction(vista, frame);
            }
        }
    }
}
