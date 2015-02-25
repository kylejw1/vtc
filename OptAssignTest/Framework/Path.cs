using System;
using System.Drawing;
using VTC.Settings;

namespace OptAssignTest.Framework
{
    public class Path
    {
        #region Inner structs

        // ER: can be replaced with some standard implementation

        public struct Vector
        {
            public readonly double X;
            public readonly double Y;
            public static readonly Vector Zero = new Vector(0, 0);

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

            /// <summary>
            /// It's known that one of the vector components is zero.
            /// So length is simply the value of non-zero component.
            /// </summary>
            /// <value>Length of the vector.</value>
            public uint SimpleLength
            {
                get { return (uint) (Math.Max(Math.Abs(X), Math.Abs(Y))); }
            }

            public Vector Inverse()
            {
                return new Vector(-X, -Y);
            }
        }

        #endregion

        private readonly double _height;
        private readonly double _width;
        private readonly uint _carRadius;
        private readonly double _halfWidth;
        private readonly double _halfHeight;
        private readonly Point _center;

        // TODO: add support for speed and acceleration
        // right now it's 1px movement per frame

        private Path(double width, double height, uint carRadius)
        {
            _height = height;
            _width = width;
            _carRadius = carRadius;
            _halfWidth = _width/2;
            _halfHeight = _height/2;
            _center = new Point((int) _halfWidth, (int) _halfHeight);
        }

        public static Path New(ISettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new Path(settings.FrameWidth, settings.FrameHeight, (uint) settings.CarRadius);
        }

        /// <summary>
        /// Generate path for horizontal or vertical movement through whole scene via center point.
        /// </summary>
        /// <param name="fromDirection">'from' direction.</param>
        /// <param name="offset">Offset from the default path.</param>
        public IPathGenerator StraightFrom(Direction fromDirection, Vector? offset = null)
        {
            // calculate start point
            Vector dirFrom = VectorFrom(fromDirection);
            var scaledDir = dirFrom.Scaled(_halfWidth, _halfHeight);
            Point fromPoint = _center - scaledDir;

            // find length of visible path
            uint distance = 2 * scaledDir.SimpleLength;

            SectionPath path = new SectionPath();
            path.AddSegment(/*0 + */_carRadius, distance - 1, frame => fromPoint + dirFrom.Scaled(frame, frame), offset);  
            return path;
        }

        public IPathGenerator EnterAndTurn(Direction from, Direction turn, Vector? offset = null)
        {
            // special handling for straight movement
            if (from == turn) return StraightFrom(from);

            // calculate start point
            Vector dirFrom = VectorFrom(from);
            var scaledFrom = dirFrom.Scaled(_halfWidth, _halfHeight);
            Point fromPoint = _center - scaledFrom;

            // find length of first path section
            uint firstHalf = scaledFrom.SimpleLength;


            // calculate path section after turn
            Vector dirAfterTurn = VectorTo(turn);
            var scaledAfterTurn = dirAfterTurn.Scaled(_halfWidth, _halfHeight);
            uint secondHalf = scaledAfterTurn.SimpleLength;

            // generate path
            SectionPath path = new SectionPath();
            path.AddSegment(/*0 + */_carRadius, firstHalf - 1, frame => fromPoint + dirFrom.Scaled(frame, frame), offset);
            path.AddSegment(firstHalf, firstHalf + secondHalf, frame => _center + dirAfterTurn.Scaled(frame - firstHalf, frame - firstHalf), offset);

            return path;
        }

        /// <summary>
        /// Get 'from' unit vector for the given direction.
        /// </summary>
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

        /// <summary>
        /// Get 'to' unit vector for the given direction.
        /// </summary>
        private static Vector VectorTo(Direction direction)
        {
            return VectorFrom(direction).Inverse();
        }
    }
}