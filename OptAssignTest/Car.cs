using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OptAssignTest
{
    public class Car
    {

        #region Inner declarations

        // TODO: think - might be moved out for complex trajectories

        private class PathSection
        {
            private readonly uint _from;
            private readonly uint _to;
            private readonly Func<uint, Point> _pathGenerator;

            public PathSection(uint @from, uint to, Func<uint, Point> pathGenerator)
            {
                _from = @from;
                _to = to;
                _pathGenerator = pathGenerator;
            }

            internal bool Contains(uint frame)
            {
                return (frame >= _from) && (frame <= _to); // TODO: think - should it be strict for both sides?
            }

            internal Point GetPoint(uint frame)
            {
                if (Contains(frame)) ;
                return _pathGenerator(frame);
            }
        }

        #endregion

        private static readonly Bgr _whiteColor = new Bgr(Color.White);

        private static readonly Func<uint, Bgr> AlwaysWhiteFunc = _ => _whiteColor;
        private static readonly Func<uint, bool> AlwaysVisibleFunc = _ => true;

        private static Func<uint, Bgr> _carColor = AlwaysWhiteFunc; // car is white by default
        private Func<uint, bool> _visibilityFunc = AlwaysVisibleFunc; // car is always visible by default
        private readonly List<PathSection> _sections = new List<PathSection>();

        /// <summary>
        /// Last frame for the car.
        /// </summary>
        public uint MaxFrame
        {
            get { return _maxFrame; }
        }
        private uint _maxFrame = UInt32.MinValue;

        /// <summary>
        /// Car size.
        /// </summary>
        public uint CarRadius
        {
            get { return _carRadius; }
        }
        private readonly uint _carRadius;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="carRadius">Car size.</param>
        public Car(uint carRadius) 
        {
            _carRadius = carRadius;
        }

        /// <summary>
        /// Set car as always visible.
        /// </summary>
        public Car AlwaysVisible()
        {
            return Visibility(AlwaysVisibleFunc);
        }

        /// <summary>
        /// Set custom visibility for the car.
        /// </summary>
        /// <param name="visibilityFunc">
        /// Function which returns visibility for the car for the given frame. 
        /// </param>
        public Car Visibility(Func<uint, bool> visibilityFunc)
        {
            _visibilityFunc = visibilityFunc;
            return this;
        }

        /// <summary>
        /// Set custom car coloring for the car.
        /// </summary>
        /// <param name="colorFunc">Function which returns color of the car for the given frame.</param>
        public Car CarColor(Func<uint, Bgr> colorFunc)
        {
            _carColor = colorFunc;
            return this;
        }

        /// <summary>
        /// Set custom car coloring for the car.
        /// </summary>
        public Car CarColor(Color color)
        {
            return CarColor(new Bgr(color));
        }

        /// <summary>
        /// Set custom car coloring for the car.
        /// </summary>
        public Car CarColor(Bgr color)
        {
            return CarColor(_ => color);
        }

        public Car AddPath(uint @from, uint to, Func<uint, Point> pathGenerator)
        {
            // validate inputs
            if (pathGenerator == null) throw new ArgumentNullException("pathGenerator");
            if (@from >= to) throw new ArgumentException("Path definition is wrong");

            if (_sections.Any(pathSection => pathSection.Contains(@from) || pathSection.Contains(to)))
                throw new ArgumentException("Segment intersects with another segment.");

            // register new path section
            var section = new PathSection(@from, to, pathGenerator);
            _sections.Add(section);

            _maxFrame = Math.Max(MaxFrame, to);

            return this;
        }

        public void Draw(uint frame, Image<Bgr, byte> scene)
        {
            // must be visible
            if (! _visibilityFunc(frame)) return;

            foreach (var section in _sections.Where(s => s.Contains(frame))) // ER: can be optimized if list is sorted
            {
                // draw car at the point
                var point = section.GetPoint(frame);
                scene.Draw(new CircleF(new PointF(point.X, point.Y), CarRadius), _carColor(frame), 0); // TODO: someday move out actual drawing out of here

                break;
            }
        }
    }
}