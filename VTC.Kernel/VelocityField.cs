using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using VTC.Kernel.Extensions;

namespace VTC.Kernel
{
    public class VelocityField
    {
        public struct Velocity
        {
            public double v_x;
            public double v_y;

            public Velocity(double x, double y)
            {
                v_x = x;
                v_y = y;
            }
        }

        private Dictionary<int, int> _horizontalCoordLookup = new Dictionary<int,int>();
        private Dictionary<int, int> _verticalCoordLookup = new Dictionary<int, int>();
        private Velocity[,] _velocityField;
        private int _fieldHeight;
        private int _fieldWidth;
        private int _sourceWidth;
        private int _sourceHeight;
        private double alpha = 0.01;
        private Mutex _updateMutex = new Mutex();

        public VelocityField(int fieldWidth, int fieldHeight, int sourceWidth, int sourceHeight)
        {
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;
            _sourceHeight = sourceHeight;
            _sourceWidth = sourceWidth;

            _velocityField = new Velocity[_fieldWidth, _fieldHeight];
        }

        public Velocity GetAvgVelocity(Point p)
        {
            return _velocityField[p.X, p.Y];
        }

        private void InsertVelocities(IEnumerable<Tuple<Point, Velocity>> measurements)
        {
            if (!_updateMutex.WaitOne(0))
            {
                return;
            }

            try
            {
                var neighbors = new List<Point>();
                var velocities = new List<Velocity>();

                foreach (var m in measurements)
                {
                    // Cache the normalized values for efficiency
                    int x, y;
                    if (!_horizontalCoordLookup.ContainsKey(m.Item1.X))
                    {
                        _horizontalCoordLookup[m.Item1.X] = (m.Item1.X * _fieldWidth) / _sourceWidth;
                    }

                    x = _horizontalCoordLookup[m.Item1.X];

                    if (!_verticalCoordLookup.ContainsKey(m.Item1.Y))
                    {
                        _horizontalCoordLookup[m.Item1.Y] = (m.Item1.Y * _fieldHeight) / _sourceHeight;
                    }

                    y = _horizontalCoordLookup[m.Item1.Y];

                    neighbors.Add(new Point(x,y));
                    velocities.Add(m.Item2);
                }

                if (!neighbors.Any())
                    return;

                var point = new Point();
                for (int x = 0; x < _fieldWidth; x++)
                {
                    point.X = x;
                    for (int y = 0; y < _fieldHeight; y++)
                    {
                        point.Y = y;

                        var nearest = point.NearestNeighborIndex(neighbors);

                        var vx = _velocityField[x, y].v_x*(1 - alpha);
                        vx += (velocities[nearest].v_x*alpha);

                        var vy = _velocityField[x, y].v_y*(1 - alpha);
                        vy += (velocities[nearest].v_y*alpha);

                        _velocityField[x, y].v_x = vx;
                        _velocityField[x, y].v_y = vy;
                    }
                }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
        }

        internal void TryInsertVelocitiesAsync(IEnumerable<Tuple<Point, Velocity>> measurements)
        {
            Task.Factory.StartNew(() => InsertVelocities(measurements.ToList()));
        }

        public void Draw<TColor, TDepth>(Emgu.CV.Image<TColor, TDepth> image, TColor color, int thickness) where TColor : struct, IColor where TDepth : new()
        {
            _updateMutex.WaitOne();
            try
            {
                var segmentWidth = image.Width/_fieldWidth;
                var segmentHeight = image.Height/_fieldHeight;

                var cursorStart = new Point(segmentWidth/2, segmentHeight/2);

                for (int x = 0; x < _fieldWidth; x++)
                {
                
                    for (int y = 0; y < _fieldHeight; y++)
                    {
                        var cursorEnd = new Point(
                            (int) (cursorStart.X + _velocityField[x, y].v_x),
                            (int) (cursorStart.Y + _velocityField[x, y].v_y)
                            );

                        var line = new LineSegment2D(cursorStart, cursorEnd);

                        image.Draw(line, color, thickness);
                        image.Draw(new CircleF(cursorEnd, 3), color, thickness);

                        cursorStart.Y += segmentHeight;
                    }

                    cursorStart.Y = segmentHeight / 2;
                    cursorStart.X += segmentWidth;
                }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
        }
    } 
}
