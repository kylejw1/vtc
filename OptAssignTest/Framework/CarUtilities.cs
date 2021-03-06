﻿using System;
using VTC.Common;

namespace OptAssignTest.Framework
{
    public static class CarUtilities
    {
        public static Car AddVerticalPath(this Car car, ISettings settings, Direction @from = Direction.South, Path.Vector? offset = null)
        {
            if (! (from == Direction.South || from == Direction.North)) 
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = Path.New(settings).StraightFrom(from, offset);
            return car.SetPath(path);
        }

        public static Car AddHorizontalPath(this Car car, ISettings settings, Direction from = Direction.East, Path.Vector? offset = null)
        {
            if (!(from == Direction.East || from == Direction.West))
                throw new ArgumentOutOfRangeException("from", "Wrong direction");

            var path = Path.New(settings).StraightFrom(from, offset);
            return car.SetPath(path);
        }

        public static Car AddTurn(this Car car, ISettings settings, Direction from, Direction turn, Path.Vector? offset = null)
        {
            var path = Path
                        .New(settings)
                        .EnterAndTurn(from, turn, offset);

            return car.SetPath(path);
        }

        public static Car StraightPathFrom(this Car car, ISettings settings, Direction from, Path.Vector? offset = null)
        {
            var path = Path.New(settings).StraightFrom(from, offset);
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
