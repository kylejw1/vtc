﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VTC
{
    public class Polygon : List<Point>
    {
        public bool PolygonClosed
        {
            get
            {
                if (this.Count() < 3) return false;

                return (this.First().Equals(this.Last()));
            }
        }

        public Image<Bgr, float> GetMask(int width, int height, Bgr fgColor)
        {
            var coords = new List<Point>();

            // Can't simply use fillConvexPoly here as we expect that the poly is not
            // usually convex.  Instead, draw the poly and use the flood tool to fill in the rest of the image with black

            // In order for flood tool not to mess up if roi extends from one end to another, move any points on the border inwards
            // by one pixel.  This allows a buffer for the flood tool to travel around.
            foreach (var coord in this)
            {
                var x = coord.X;
                var y = coord.Y;

                if (x <= 0) x = 1;
                if (y <= 0) y = 1;
                if (x >= width) x = width - 1;
                if (y >= height) y = height - 1;

                coords.Add(new Point(x, y));
            }

            var image = new Image<Bgr, float>(width, height);
            image.SetValue(fgColor);

            image.DrawPolyline(coords.ToArray(), true, new Bgr(Color.Black), 1);

            MCvConnectedComp comp = new MCvConnectedComp();

            MCvScalar lo = new MCvScalar(1, 1, 1);
            MCvScalar up = new MCvScalar(1, 1, 1);

            // Know it is safe to fill at 0,0 because we created a 1 pixel buffer around all coordinates
            CvInvoke.cvFloodFill(image.Ptr,
                new Point(0, 0),
                new MCvScalar(0, 0, 0),
                lo,
                up,
                out comp,
                Emgu.CV.CvEnum.CONNECTIVITY.FOUR_CONNECTED,
                Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
                new IntPtr());

            return image;
        }
    }
}