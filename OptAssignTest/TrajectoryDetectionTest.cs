using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTC;
using VTC.Settings;

namespace OptAssignTest
{
    [TestClass]
    public class TrajectoryDetectionTest
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
            const int vehicleRadius = 3; // in pixels
            ISettings settings = CreateSettings(vehicleRadius);
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
            const int vehicleRadius = 3; // in pixels
            ISettings settings = CreateSettings(vehicleRadius);
            var vista = CreateVista(settings);

            var diagonals = Enumerable.Range(5 + vehicleRadius, 195).Select(x => new[] { new Point(x, x + 5), new Point(x, x - 5) });

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

        /// <summary>
        /// Creates initialized intersection vista to be used for tests.
        /// </summary>
        /// <returns></returns>
        private static IntersectionVista CreateVista(ISettings settings)
        {
            // create mask for the whole image
            var polygon = new Polygon();
            polygon.AddRange(new[]
                            {
                                new Point(0, 0), 
                                new Point(0, (int) settings.FrameHeight),
                                new Point((int) settings.FrameWidth, (int) settings.FrameHeight), 
                                new Point((int) settings.FrameWidth, 0),
                                new Point(0, 0)
                            });

            var regionConfig = new RegionConfig
                                {
                                    RoiMask = polygon
                                };

            return new IntersectionVista(settings, (int) settings.FrameWidth, (int) settings.FrameHeight)
                    {
                        RegionConfiguration = regionConfig
                    };
        }

        /// <summary>
        /// Assuming that vehicles is circle, create settings for it.
        /// </summary>
        /// <param name="vehicleRadius">Radius of "vehicle"</param>
        private static ISettings CreateSettings(int vehicleRadius)
        {
            return new TestSettings { CarRadius = vehicleRadius };
        }
    }
} ;
