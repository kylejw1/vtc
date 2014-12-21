using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTC.Settings;

namespace OptAssignTest
{
    [TestClass]
    public class TrajectoryDetectionTest : TestBase
    {
        [TestMethod]
        public void EmptyTrajectory_ShouldNotDetectVehicles()
        {
            var settings = new TestSettings();
            var vista = CreateVista(settings);

            var generator = new CircleVehicles((int)settings.FrameWidth, (int)settings.FrameHeight, Enumerable.Empty<Point[]>());
            foreach (var frame in generator.Frames())
            {
                vista.Update(frame);
                Assert.AreEqual(0, vista.LastDetectionCount); 
            }
        }

        [TestMethod]
        public void SingleDiagonalTrajectory_ShouldDetectSingleVehicle()
        {
            ISettings settings = CreateSettings(VehicleRadius);
            var vista = CreateVista(settings);

            var diagonal = Enumerable.Range(5, 195).Select(x => new[] { new Point(x, x) });

            int count = 0;
            var generator = new CircleVehicles((int) settings.FrameWidth, (int) settings.FrameHeight, diagonal);
            foreach (var frame in generator.Frames())
            {
                vista.Update(frame);
                if (count++ > 2) // ignoring first iterations, since it saves background (first step) and accumulates statistics(?).
                {
                    Assert.AreEqual(1, vista.CurrentVehicles.Count);
                }
            }
        }
        
        [TestMethod]
        public void TwoDiagonalsTrajectories_ShouldDetectTwoVehicles()
        {
            ISettings settings = CreateSettings(VehicleRadius);
            var vista = CreateVista(settings);

            var diagonals = Enumerable.Range(5 + VehicleRadius, 195).Select(x => new[] { new Point(x, x + 5), new Point(x, x - 5) });

            int count = 0;
            var generator = new CircleVehicles((int) settings.FrameWidth, (int) settings.FrameHeight, diagonals);
            foreach (var frame in generator.Frames())
            {
//                frame.Save(@"c:\temp\frame" + count + ".png");
                vista.Update(frame);
                if (count++ > 2) // ignoring first iterations, since it saves background (first step) and accumulates statistics(?).
                {
                    Assert.AreEqual(2, vista.CurrentVehicles.Count);
                }
            }
        }
    }
} ;
