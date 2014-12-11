using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTC;

namespace OptAssignTest
{
    [TestClass]
    public class TrajectoryDetectionTest
    {
        // shared settings
        private static readonly TestSettings _settings = new TestSettings();

        [TestMethod]
        public void EmptyTrajectory_ShouldNotDetectVehicles()
        {
            var vista = CreateVista();

            var generator = new CircleVehicles((int)_settings.FrameWidth, (int)_settings.FrameHeight, Enumerable.Empty<Point[]>());
            foreach (var frame in generator.Frames())
            {
                vista.Update(frame);
                Assert.AreEqual(0, vista.LastDetectionCount); 
            }
        }

        [TestMethod]
        public void SingleDiagonalTrajectory_ShouldDetectSingleVehicle()
        {
            var vista = CreateVista();

            var diagonal = Enumerable.Range(5, 195).Select(x => new[] { new Point(x, x) });

            int count = 0;
            var generator = new CircleVehicles((int) _settings.FrameWidth, (int) _settings.FrameHeight, diagonal);
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
            var vista = CreateVista();

            var diagonals = Enumerable.Range(5, 195).Select(x => new[] { new Point(x, x + 5), new Point(x, x - 5) });

            int count = 0;
            var generator = new CircleVehicles((int) _settings.FrameWidth, (int) _settings.FrameHeight, diagonals);
            foreach (var frame in generator.Frames())
            {
                vista.Update(frame);
                if (count++ > 5) // ignoring first iterations, since it saves background (first step) and accumulates statistics(?).
                {
                    Assert.AreEqual(2, vista.CurrentVehicles.Count,"Expected: 2, Actual: " + vista.CurrentVehicles.Count + " Failed on step " + count.ToString() );
                }
            }
        }

        /// <summary>
        /// Creates initialized intersection vista to be used for tests.
        /// </summary>
        /// <returns></returns>
        private static IntersectionVista CreateVista()
        {
            // create mask for the whole image
            var polygon = new Polygon();
            polygon.AddRange(new[]
                            {
                                new Point(0, 0), 
                                new Point(0, (int) _settings.FrameHeight),
                                new Point((int) _settings.FrameWidth, (int) _settings.FrameHeight), 
                                new Point((int) _settings.FrameWidth, 0),
                                new Point(0, 0)
                            });

            var regionConfig = new RegionConfig
                                {
                                    RoiMask = polygon
                                };

            return new IntersectionVista(_settings, (int) _settings.FrameWidth, (int) _settings.FrameHeight)
                    {
                        RegionConfiguration = regionConfig
                    };
        }
    }
} ;
