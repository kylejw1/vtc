using System;
using VTC.Settings;

namespace OptAssignTest.Framework
{
    public static class CarUtilities
    {
        public static Car AddVerticalPath(this Car car, ISettings settings, Direction from = Direction.South)
        {
            if (! (from == Direction.South || from == Direction.North)) 
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = PathCreator.New(settings).StraightFrom(from);
            return car.SetPath(path);
        }

        public static Car AddHorizontalPath(this Car car, ISettings settings, Direction from = Direction.East)
        {
            if (!(from == Direction.East || from == Direction.West))
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = PathCreator.New(settings).StraightFrom(from);
            return car.SetPath(path);
        }

        public static Car StraightPathFrom(this Car car, ISettings settings, Direction from)
        {
            var path = PathCreator.New(settings).StraightFrom(from);
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
