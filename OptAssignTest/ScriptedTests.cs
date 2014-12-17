using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void VisibilityLoss_ShouldBeDetected()
        {
            const uint frameWhenDetectionLost = 150;
            const int vehicleRadius = 3;

            var settings = CreateSettings(vehicleRadius);
            var vista = CreateVista(settings);

            var script = new Script();
            script
                .CreateCar()
                .AddPath(0, 400, frame => new Point((int) settings.FrameWidth / 2, (int)frame + vehicleRadius)) // vertical path
                .Visibility(frame => frame < frameWhenDetectionLost); // car visible only in beginning

            var background = new Image<Bgr, byte>((int) settings.FrameWidth, (int) settings.FrameHeight, new Bgr(Color.Black));
            vista.Update(background);

            var lastFrame = script.MaxFrame;
            for (uint frame = 0; frame < lastFrame; frame++)
            {
                var image = background.Clone();

                script.Draw(frame, image);
                vista.Update(image);

                // the car should be detected at that point
                if (frame == DetectionThreshold)
                {
                    Assert.AreEqual(script.Cars.Count, vista.CurrentVehicles.Count, "Car still not detected.");
                }

                // car should became invisible, but still be tracked 
                if (frame == frameWhenDetectionLost + DetectionThreshold)
                {
                    // car still should be detected
                    Assert.AreEqual(script.Cars.Count, vista.CurrentVehicles.Count, "Car still should be detected");
                    
                    // it should be detected that car is not visible anymore
                    Assert.IsTrue(vista.CurrentVehicles[0].state_history.Last().missed_detections > 0, "Car visibility loss should be detected.");
                }

                if (frame == frameWhenDetectionLost + settings.MissThreshold + DetectionThreshold)
                {
                    Assert.AreEqual(0, vista.CurrentVehicles.Count, "No cars should be detected after certain number of misses");
                }
            }
        }
    }
}
