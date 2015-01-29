using System;
using VTC.Settings;

namespace OptAssignTest.Framework
{
    public static class CarUtilities
    {
        public static Car AddVerticalPath(this Car car, ISettings settings, Direction from)
        {
            if (! (from == Direction.South || from == Direction.North)) 
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = Path.New(settings).StraightFrom(from);
            return car.SetPath(path);
        }

        public static Car AddHorizontalPath(this Car car, ISettings settings, Direction from)
        {
            if (!(from == Direction.East || from == Direction.West))
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = Path.New(settings).StraightFrom(from);
            return car.SetPath(path);
        }

        public static Car AddTurn(this Car car, ISettings settings, Direction from, Direction turn)
        {
            var path = Path
                        .New(settings)
                        .EnterAndTurn(from, turn);

            return car.SetPath(path);
        }

        public static Car StraightPathFrom(this Car car, ISettings settings, Direction from)
        {
            var path = Path.New(settings).StraightFrom(from);
            return car.SetPath(path);
        }


        public static uint VerticalPathLength(this ISettings settings) // TODO: should be merged to path generator.
        {
            return (uint) settings.FrameHeight;
        }

        public static uint HorizontalPathLength(this ISettings settings) // TODO: should be merged to path generator.
        {
            return (uint) settings.FrameWidth;
        }
    }
}
