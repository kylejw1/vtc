using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.Serialization;
using NetTopologySuite;


namespace VTC.Kernel.RegionConfig
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

        public Point Centroid;

        public void UpdateCentroid()
        {
            var points = this.Select(x => new GeoAPI.Geometries.Coordinate(x.X, x.Y)).ToArray();
            NetTopologySuite.Geometries.LinearRing ring = new NetTopologySuite.Geometries.LinearRing(points);
            NetTopologySuite.Geometries.Polygon ntsPoly = new NetTopologySuite.Geometries.Polygon(ring);
            NetTopologySuite.Algorithm.Centroid ntsCentroid = new NetTopologySuite.Algorithm.Centroid(ntsPoly);
            Centroid.X = (int)ntsCentroid.GetCentroid().X;
            Centroid.Y = (int)ntsCentroid.GetCentroid().Y;
            return;
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
            //CvInvoke.cvFloodFill(image.Ptr,
            //    new Point(0, 0),
            //    new MCvScalar(0, 0, 0),
            //    lo,
            //    up,
            //    out comp,
            //    Emgu.CV.CvEnum.CONNECTIVITY.FOUR_CONNECTED,
            //    Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
            //    new IntPtr());
            IInputOutputArray iioArray = image;
            var rect = new Rectangle();
            var ioArray = image.GetInputOutputArray();
            var mask = new Image<Gray, byte>(new Size(image.Width + 2, image.Height + 2));
            IInputOutputArray iioMask = mask;
            try
            {
                CvInvoke.FloodFill(iioArray, mask, new Point(0, 0), new MCvScalar(0, 0, 0), out rect, lo, up,
                Connectivity.FourConnected, FloodFillType.Default);
            }
            catch(Emgu.CV.Util.CvException e)
            {
                Debug.WriteLine("Exception in GetMask:CvInvoke.FloodFill");
            }
            

            return image;
        }
    }
}
