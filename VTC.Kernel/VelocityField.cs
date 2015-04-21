using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VTC.Kernel.Extensions;

namespace VTC.Kernel
{
    public class VelocityField
    {
        private int[] _horizontalCoordLookup;
        private int[] _verticalCoordLookup;
        private double[,] _velocityField;
        private int _height;
        private int _width;
        private double alpha = 0.01;

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

            _velocityField = new double[width, height];
            _width = width;
            _height = height;

        }

        public double GetAvgVelocity(Point p)
        {
            return _velocityField[p.X, p.Y] / 100;
        }

        public void InsertMeasurements(IEnumerable<Tuple<Point, double>> measurements)
        {
            var neighbors = measurements.Select(m => m.Item1).ToList();

            var point = new Point();
            for (int x = 0; x < _width; x++)
            {
                point.X = x;
                for (int y = 0; y < _height; y++)
                {
                    point.Y = y;

                    var nearest = point.NearestNeighborIndex(neighbors);

                    _velocityField[x,y] *= (1 - alpha);

                }
            }

            var x = _horizontalCoordLookup[p.X];
            var y = _horizontalCoordLookup[p.Y];


        }
    } 
}
