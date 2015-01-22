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

        [TestMethod]
        [Description("Empty script should behave as a completed script.")]
        public void EmptyScriptCompleteness() 
        {
            var script = new Script();

            Assert.IsTrue(script.IsDone(0), "Should be done for beginning.");
        }
    }
}
