using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OptAssignTest.Framework
{
    public class Car
    {
        private static readonly Bgr _whiteColor = new Bgr(Color.White);

        private static readonly Func<uint, Bgr> AlwaysWhiteFunc = _ => _whiteColor;
        private static readonly Func<uint, bool> AlwaysVisibleFunc = _ => true;

        private static Func<uint, Bgr> _carColor = AlwaysWhiteFunc; // car is white by default
        private Func<uint, bool> _visibilityFunc = AlwaysVisibleFunc; // car is always visible by default

        private IPathGenerator _path;

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

        public Car SetPath(IPathGenerator path)
        {
            if (path == null) throw new ArgumentNullException("path");
            _path = path;
            return this;
        }

        public void Draw(uint frame, Image<Bgr, byte> scene)
        {
            // must be visible
            if (! _visibilityFunc(frame)) return;

            Point? position = _path.GetPosition(frame);
            if (position.HasValue)
            {
                scene.Draw(new CircleF(new PointF(position.Value.X, position.Value.Y), CarRadius), _carColor(frame), 0); // TODO: someday move out actual drawing out of here
            }
        }

        public bool IsDone(uint frame)
        {
            return _path.IsDone(frame);
        }
    }
}