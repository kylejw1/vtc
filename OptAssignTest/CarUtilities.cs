using System.Drawing;
using VTC.Settings;

namespace OptAssignTest
{
    public static class CarUtilities
    {
        public static Car AddVerticalPath(this Car car, ISettings settings)
        {
            var midX = (int)settings.FrameWidth / 2;
            var pathLength = (uint)settings.FrameHeight - car.CarRadius;

            return car.AddPath(0, pathLength, frame => new Point(midX, (int)(frame + car.CarRadius)));
        }
    }
}
