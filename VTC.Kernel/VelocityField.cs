using System;
using System.CodeDom;
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
        private double alpha = 0.001;
        private int distance_threshold = 2;
        private Mutex _updateMutex = new Mutex();

        public Image<Gray, Byte> ProjectedPointsImage;

        public VelocityField(int fieldWidth, int fieldHeight, int sourceWidth, int sourceHeight)
        {
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;
            _sourceHeight = sourceHeight;
            _sourceWidth = sourceWidth;

            _velocityField = new Velocity[_fieldWidth, _fieldHeight];
            ProjectedPointsImage = new Image<Gray, byte>(_fieldWidth, _fieldHeight, new Gray(0));
        }

        public Velocity GetAvgVelocity(int x, int y)
        {
            GetNormalizedCoordinate(x, y, out x, out y);
            return _velocityField[x, y];
        }

        public Velocity GetAvgVelocity(Point p)
        {
            return GetAvgVelocity(p.X, p.Y);
        }

        private void GetNormalizedCoordinate(int x, int y, out int xNormal, out int yNormal)
        {
            // Cache the normalized values to reduce multiplication and division
            //if (!_horizontalCoordLookup.ContainsKey(x))
            //{
            //    var value = (x * _fieldWidth) / _sourceWidth;
            //    value = Math.Min(value, _fieldWidth-1);
            //    _horizontalCoordLookup[x] = value;
            //}

            //xNormal = _horizontalCoordLookup[x];
            xNormal = (x * _fieldWidth) / _sourceWidth;
            xNormal = Math.Min(xNormal, _fieldWidth - 1);

            //if (!_verticalCoordLookup.ContainsKey(y))
            //{
            //    var value = (y * _fieldHeight) / _sourceHeight;
            //    value = Math.Min(value, _fieldHeight-1);
            //    _horizontalCoordLookup[y] = value;
            //}

            //yNormal = _horizontalCoordLookup[y];
            yNormal = (y * _fieldHeight) / _sourceHeight;
            yNormal = Math.Min(yNormal, _fieldHeight - 1);
        }

        private void InsertVelocities(Dictionary<Point, Velocity> measurements)
        {
            // If we're already busy inserting velocities, just abort
            if (!_updateMutex.WaitOne(0))
            {
                return;
            }

            try
            {
                var neighbors = new List<Point>();
                var velocities = new List<Velocity>();
                ProjectedPointsImage = new Image<Gray, byte>(_fieldWidth, _fieldHeight, new Gray(0));

                // Since the velocity field grid is smaller that the source image, we need to
                // Normalize the measurement coordinates
                foreach (var kvp in measurements)
                {
                    var pt = kvp.Key;
                    var vel = kvp.Value;

                    int x, y;
                    GetNormalizedCoordinate(pt.X, pt.Y, out x, out y);

                    neighbors.Add(new Point(x,y));
                    velocities.Add(vel);
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

                        if (point.DistanceTo(neighbors[nearest]) < distance_threshold)
                        {
                            if (x == 32 && y == 20)
                                Console.WriteLine("This point");

                            var vx = _velocityField[x, y].v_x * (1 - alpha);
                            vx += (velocities[nearest].v_x * alpha);

                            var vy = _velocityField[x, y].v_y * (1 - alpha);
                            vy += (velocities[nearest].v_y * alpha);

                            _velocityField[x, y].v_x = vx;
                            _velocityField[x, y].v_y = vy;    
                        }
                        else
                        {
                            _velocityField[x, y].v_x = _velocityField[x, y].v_x*(1-alpha);
                            _velocityField[x, y].v_y = _velocityField[x, y].v_y*(1-alpha);    
                        }
                    }
                }

                foreach (var v in neighbors)
                    ProjectedPointsImage.Draw(new CircleF(new PointF(v.X, v.Y), 1), new Gray(255), 1);
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
        }

        internal void TryInsertVelocitiesAsync(Dictionary<Point, Velocity> measurements)
        {
            Task.Factory.StartNew(() => InsertVelocities(measurements));
        }

        public void Draw<TColor, TDepth>(Emgu.CV.Image<TColor, TDepth> image, TColor color, int thickness) 
            where TColor : struct, IColor 
            where TDepth : new()
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

                        if (line.Length > 1)
                        {
                            image.Draw(line, color, thickness);
                            image.Draw(new CircleF(cursorStart, 1), color, thickness);    
                        }

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
