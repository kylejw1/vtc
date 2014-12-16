using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OptAssignTest
{
    class Car
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

        private static readonly Bgr _carColor = new Bgr(Color.White);
        private static readonly Func<uint, bool> AlwaysVisibleFunc = _ => true;

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
        /// Function which returns opacity for the car for the given frame. 
        /// 1 is full opacity, 0 is full transparency.
        /// </param>
        public Car Visibility(Func<uint, bool> visibilityFunc)
        {
            _visibilityFunc = visibilityFunc;
            return this;
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
                scene.Draw(new CircleF(new PointF(point.X, point.Y), 3), _carColor, 0);

                break;
            }
        }
    }
}