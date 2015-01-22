﻿using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptAssignTest
{
    /// <summary>
    /// Self tests for test helpers.
    /// </summary>
    [TestClass]
    public class PathCreatorTests : ScriptedTestBase
    {
        [TestMethod]
        [Description("Path with single path section should detect section bounds correctly.")]
        public void SinglePathCompleteness() 
        {
            var settings = CreateSettings(VehicleRadius);

            // create generator with vertical path
            IPathGenerator generator = PathCreator
                                        .New(settings)
                                        .StraightFrom(Direction.South);

            Assert.IsFalse(generator.IsDone(0), "Should not be done for beginning.");
            Assert.IsFalse(generator.IsDone(settings.VerticalPathLength() - 1), "Should not be done for last point.");
            Assert.IsTrue(generator.IsDone(settings.VerticalPathLength()), "Should be done for next after the last point.");
        }

        [TestMethod]
        [Description("Path with straight South->North path section should detect section bounds correctly.")]
        public void ScriptWithSingleSouthPathCompleteness()
        {
            SingleVerticalPathTest(Direction.South);
        }

        [TestMethod]
        [Description("Path with straight North->South path section should detect section bounds correctly.")]
        public void ScriptWithSingleNorthPathCompleteness() 
        {
            SingleVerticalPathTest(Direction.North);
        }

        [TestMethod]
        [Description("Path with straight West->East path section should detect section bounds correctly.")]
        public void ScriptWithSingleWestPathCompleteness() 
        {
            SingleHorizontalPathTest(Direction.West);
        }

        [TestMethod]
        [Description("Path with straight East->West path section should detect section bounds correctly.")]
        public void ScriptWithSingleEastPathCompleteness() 
        {
            SingleHorizontalPathTest(Direction.East);
        }

        [TestMethod]
        [Description("Check points generated for South->North path.")]
        public void FromSouthDirection()
        {
            VerifyDirection(Direction.South, delegate(Point currPos, Point prevPos)
                                                {
                                                    Assert.AreEqual(currPos.X, prevPos.X, "No change in X position is expected.");
                                                    Assert.IsTrue(currPos.Y > prevPos.Y, "The path should go up.");
                                                });
        }

        [TestMethod]
        [Description("Check points generated for North->South path.")]
        public void FromNorthDirection()
        {
            VerifyDirection(Direction.North, delegate(Point currPos, Point prevPos)
                                                {
                                                    Assert.AreEqual(currPos.X, prevPos.X, "No change in X position is expected.");
                                                    Assert.IsTrue(currPos.Y < prevPos.Y, "The path should go down.");
                                                });
        }

        [TestMethod]
        [Description("Check points generated for East->West path.")]
        public void FromEastDirection()
        {
            VerifyDirection(Direction.East, delegate(Point currPos, Point prevPos)
                                                {
                                                    Assert.AreEqual(currPos.Y, prevPos.Y, "No change in Y position is expected.");
                                                    Assert.IsTrue(currPos.X < prevPos.X, "The path should go left.");
                                                });
        }

        [TestMethod]
        [Description("Check points generated for West->East path.")]
        public void FromWestDirection()
        {
            VerifyDirection(Direction.West, delegate(Point currPos, Point prevPos)
                                                {
                                                    Assert.AreEqual(currPos.Y, prevPos.Y, "No change in Y position is expected.");
                                                    Assert.IsTrue(currPos.X > prevPos.X, "The path should go right.");
                                                });
        }

        private static void VerifyDirection(Direction direction, Action<Point, Point> validation)
        {
            var settings = CreateSettings(VehicleRadius);

            IPathGenerator generator = PathCreator
                .New(settings)
                .StraightFrom(direction);

            Point? prevPoint = null;
            for (uint frame = 0; !generator.IsDone(frame); frame++)
            {
                Point? position = generator.GetPosition(frame);
                if (! position.HasValue) continue;

                if (prevPoint.HasValue)
                {
                    validation(position.Value, prevPoint.Value);
                }
                prevPoint = position;
            }
        }

        [TestMethod]
        [Description("Empty script should behave as a completed script.")]
        public void EmptyScriptCompleteness() 
        {
            var script = new Script();

            Assert.IsTrue(script.IsDone(0), "Should be done for beginning.");
        }

        private static void SingleVerticalPathTest(Direction direction)
        {
            var settings = CreateSettings(VehicleRadius);

            // create generator with vertical path
            Script script = new Script();
            script.CreateCar(VehicleRadius).AddVerticalPath(settings, direction);

            Assert.IsFalse(script.IsDone(0), "Should not be done for beginning.");
            Assert.IsFalse(script.IsDone(settings.VerticalPathLength() - 1), "Should not be done for last point.");
            Assert.IsTrue(script.IsDone(settings.VerticalPathLength()), "Should be done for next after the last point.");
        }

        private static void SingleHorizontalPathTest(Direction direction)
        {
            var settings = CreateSettings(VehicleRadius);

            // create generator with horizontal path
            Script script = new Script();
            script.CreateCar(VehicleRadius).AddHorizontalPath(settings, direction);

            Assert.IsFalse(script.IsDone(0), "Should not be done for beginning.");
            Assert.IsFalse(script.IsDone(settings.HorizontalPathLength() - 1), "Should not be done for last point.");
            Assert.IsTrue(script.IsDone(settings.HorizontalPathLength()), "Should be done for next after the last point.");
        }
    }
}
