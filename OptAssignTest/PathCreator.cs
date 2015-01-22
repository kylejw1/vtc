using System;
using System.Drawing;
using VTC.Settings;

namespace OptAssignTest
{
    class PathCreator
    {
        #region Inner structs

        // ER: can be replaced with some standard implementation

        private struct Vector
        {
            public readonly double X;
            public readonly double Y;

            public Vector(double x, double y)
            {
                X = x;
                Y = y;
            }

            public Vector Scaled(double scaleX, double scaleY)
            {
                return new Vector(X*scaleX, Y*scaleY);
            }

            public static Point operator +(Point p, Vector v)
            {
                return new Point((int) (p.X + v.X), (int) (p.Y + v.Y));
            }

            public static Point operator -(Point p, Vector v)
            {
                return new Point((int) (p.X - v.X), (int) (p.Y - v.Y));
            }
        }

        #endregion

        private readonly double _height;
        private readonly double _width;
        private readonly uint _carRadius;

        private PathCreator(double width, double height, uint carRadius)
        {
            _height = height;
            _width = width;
            _carRadius = carRadius;
        }

        public static PathCreator New(ISettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new PathCreator(settings.FrameWidth, settings.FrameHeight, (uint) settings.CarRadius);
        }

        /// <summary>
        /// Generate path for horizontal or vertical movement through whole scene via center point.
        /// </summary>
        /// <param name="direction">Movement direction.</param>
        public IPathGenerator StraightFrom(Direction direction)
        {
            double halfWidth = _width/2;
            double halfHeight = _height/2;

            // all paths goes through the center
            Point center = new Point((int) halfWidth, (int) halfHeight);

            // calculate start point
            Vector dirFrom = VectorFrom(direction);
            var scaledDir = dirFrom.Scaled(halfWidth, halfHeight);
            Point fromPoint = center - scaledDir;

            // find length of visible path
            uint distance = (uint)(2 * Math.Max(Math.Abs(scaledDir.X), Math.Abs(scaledDir.Y)));

            SectionPath path = new SectionPath();
            path.AddSegment(_carRadius, distance - 1, frame => fromPoint + dirFrom.Scaled(frame, frame));  // TODO: add support for speed and acceleration
            return path;
        }

        private static Vector VectorFrom(Direction direction) 
        {
            switch (direction)
            {
                case Direction.North:
                    return new Vector(0, -1);
                case Direction.East:
                    return new Vector(-1, 0);
                case Direction.South:
                    return new Vector(0, 1);
                case Direction.West:
                    return new Vector(1, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }
    }
}