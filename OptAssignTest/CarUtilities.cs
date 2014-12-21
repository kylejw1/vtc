using System.Drawing;
using VTC.Settings;

namespace OptAssignTest
{
    public static class CarUtilities
    {
        // TODO: create path generator abstraction

        public static Car AddVerticalPath(this Car car, ISettings settings)
        {
            var midX = (int)settings.FrameWidth / 2;
            var pathLength = (uint)settings.FrameHeight - car.CarRadius;

            return car.AddPath(0, pathLength, frame => new Point(midX, (int)(frame + car.CarRadius)));
        }

        public static Car AddHorizontalPath(this Car car, ISettings settings)
        {
            var midY = (int)settings.FrameHeight / 2;
            var pathLength = (uint)settings.FrameWidth - car.CarRadius;

            return car.AddPath(0, pathLength, frame => new Point((int)(frame + car.CarRadius), midY));
        }


        public static uint VerticalPathLength(this ISettings settings) // TODO: should be merged to path generator.
        {
            return (uint) settings.FrameHeight;
        }
    }
}
