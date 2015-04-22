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
            public double X;
            public double Y;

            public Velocity(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private int[] _horizontalCoordLookup;
        private int[] _verticalCoordLookup;
        private Velocity[,] _velocityField;
        private int _height;
        private int _width;
        private double alpha = 0.01;
        private Mutex _updateMutex = new Mutex();

        public VelocityField(int width, int height, int imageWidth, int imageHeight)
        {
            _horizontalCoordLookup = new int[imageWidth];
            _verticalCoordLookup = new int[imageHeight];

            for (int i = 0; i < imageWidth; i++)
            {
                _horizontalCoordLookup[i] = (i * width) / imageWidth;
            }

            for (int i = 0; i < imageHeight; i++)
            {
                _verticalCoordLookup[i] = (i * height) / imageHeight;
            }

            _velocityField = new Velocity[width, height];
            _width = width;
            _height = height;

        }

        public Velocity GetAvgVelocity(Point p)
        {
            return _velocityField[p.X, p.Y];
        }

        private void InsertMeasurements(IEnumerable<Tuple<Point, Velocity>> measurements)
        {
            if (!_updateMutex.WaitOne(0))
            {
                return;
            }

            try
            {
                if (!measurements.Any())
                    return;

                var neighbors = new List<Point>();
                var velocities = new List<Velocity>();

                foreach (var m in measurements)
                {
                    try
                    {
                        neighbors.Add(new Point(_horizontalCoordLookup[m.Item1.X], _verticalCoordLookup[m.Item1.Y]));
                        velocities.Add(m.Item2);
                    }
                    catch
                    {
                        // TODO: Handle indicies outside the image width (eg. negatives)
                    }
                }

                var point = new Point();
                for (int x = 0; x < _width; x++)
                {
                    point.X = x;
                    for (int y = 0; y < _height; y++)
                    {
                        point.Y = y;

                        var nearest = point.NearestNeighborIndex(neighbors);

                        var vx = _velocityField[x, y].X*(1 - alpha);
                        vx += (velocities[nearest].X*alpha);

                        var vy = _velocityField[x, y].Y*(1 - alpha);
                        vy += (velocities[nearest].Y*alpha);

                        _velocityField[x, y].X = vx;
                        _velocityField[x, y].Y = vy;
                    }
                }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }
        }

        internal void TryInsertEventsAsync(IEnumerable<Tuple<Point, Velocity>> measurements)
        {
            Task.Factory.StartNew(() => InsertMeasurements(measurements.ToList()));
        }

        public void Draw<TColor, TDepth>(Emgu.CV.Image<TColor, TDepth> image, TColor color, int thickness) where TColor : struct, IColor where TDepth : new()
        {
            _updateMutex.WaitOne();
            try
            {
                var segmentWidth = image.Width/_width;
                var segmentHeight = image.Height/_height;

                var cursorStart = new Point(segmentWidth/2, segmentHeight/2);

                for (int x = 0; x < _width; x++)
                {
                
                    for (int y = 0; y < _height; y++)
                    {
                        var cursorEnd = new Point(
                            (int) (cursorStart.X + _velocityField[x, y].X),
                            (int) (cursorStart.Y + _velocityField[x, y].Y)
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
