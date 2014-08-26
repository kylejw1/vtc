using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VTC
{
    public static class PointExtensions
    {
        public static double DistanceTo(this Point a, Point b)
        {
            var distanceX = Math.Abs(a.X - b.X);
            var distanceY = Math.Abs(a.Y - b.Y);

            var distance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

            return distance;
        }
    }
}
