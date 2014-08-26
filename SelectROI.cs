using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VTC
{
    public partial class SelectROI : Form
    {
        private BindingList<Point> Coordinates = new BindingList<Point>();

        private Image<Bgr, float> StartImage;

        private readonly int CIRCLE_RADIUS = 5;
        private readonly int LINE_WIDTH = 2;

        private Point? MouseDownLocation = null;

        private bool PolygonClosed
        {
            get
            {
                if (Coordinates.Count() < 3) return false;

                return (Coordinates.First().Equals(Coordinates.Last()));
            }
        }

        public SelectROI(Image<Bgr,float> _bgImage)
        {
            InitializeComponent();

            StartImage = _bgImage.Clone();

            pbBgImage.Image = StartImage.ToBitmap();
            lbCoordinates.DataSource = Coordinates;

            pbBgImage.Size = StartImage.Size;
        }

        private List<LineSegment2D> GetSegments(IEnumerable<Point> coords)
        {
            var lines = new List<LineSegment2D>();

            if (coords.Count() < 2) return lines;

            for (int i = 1; i < coords.Count(); i++)
            {
                lines.Add(new LineSegment2D(coords.ElementAt(i - 1), coords.ElementAt(i)));
            }

            return lines;
        }

        private bool LineSegment2D_IntersectsAny(LineSegment2D line, IEnumerable<LineSegment2D> collection) 
        {
            foreach (var l in collection)
            {
                if (line.Equals(l)) continue;

                if (line.Crosses(l)) return true;
            }

            return false;
        }

        void DrawCoords(IEnumerable<Point> coords)
        {
            // Get line segments
            var segments = GetSegments(coords);

            // Find any intersecting lines
            var intersections = segments.Where(s => LineSegment2D_IntersectsAny(s, segments)).ToList();
            segments.RemoveAll(s => intersections.Contains(s));

            var newImage = StartImage.Clone();

            // If closed, draw green.  Otherwise, blue.  Intersections are red
            var segmentColor = PolygonClosed ? new Bgr(Color.Green) : new Bgr(Color.Blue);
            var intersectionColor = new Bgr(Color.Red);
            var circleColor = new Bgr(Color.Green);

            // Draw good line segments
            foreach (var segment in segments)
            {
                newImage.Draw(segment, segmentColor, LINE_WIDTH);
            }

            // Draw intersecting segments
            foreach (var intersection in intersections)
            {
                newImage.Draw(intersection, intersectionColor, LINE_WIDTH);
            }

            // Mark verticies with circle
            foreach (var point in coords) 
            {
                newImage.Draw(new CircleF(point, CIRCLE_RADIUS), circleColor, LINE_WIDTH);
            }

            pbBgImage.Image = newImage.ToBitmap();

            bool polygonValid = false;
            if (PolygonClosed && intersections.Count() == 0)
            {
                polygonValid = true;

                lblMessages.Text = "Click OK to accept changes.";
            }
            else if (intersections.Count() > 0)
            {
                lblMessages.Text = "Selection lines must not intersect.";
            }
            else
            {
                lblMessages.Text = "Selected ROI must be a closed shape before accepting.";
            }

            btnOk.Enabled = polygonValid;
        }

        private bool TryClosePolygon(BindingList<Point> coords, Point down, Point up)
        {
            // Close the polygon if the down and up points are close (meaning nothing was dragged), 
            // and they are close to the first coordinate in the list

            if (PolygonClosed) return false;

            if (null == coords || coords.Count() < 3) return false;

            if (down.DistanceTo(up) > CIRCLE_RADIUS) return false;

            if (up.DistanceTo(coords.First()) > CIRCLE_RADIUS) return false;

            // Close the polygon
            coords.Add(coords.First());

            return true;
        }

        private bool TryDragCoord(BindingList<Point> coords, Point start, Point end)
        {
            int indexOfExisting;
            Point coord;
            
            try
            {
                coord = coords.First(c => start.DistanceTo(c) <= CIRCLE_RADIUS);
                indexOfExisting = coords.IndexOf(coord);
            }
            catch
            {
                return false;
            }

            if (end.X < 0) end.X = 0;
            if (end.X >= pbBgImage.Image.Width) end.X = pbBgImage.Image.Width - 1;
            if (end.Y < 0) end.Y = 0;
            if (end.Y >= pbBgImage.Image.Height) end.Y = pbBgImage.Image.Height - 1;

            coord.X = end.X;
            coord.Y = end.Y;
            coords.RemoveAt(indexOfExisting);
            coords.Insert(indexOfExisting, coord);

            return true;
        }

        private void pbBgImage_MouseUp(object sender, MouseEventArgs e)
        {
            // Discard errant mouseups
            if (null == MouseDownLocation) return;

            var mouseUpLocation = new Point(e.Location.X, e.Location.Y);

            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    {
                        // If clicked on first point, close polygon
                        if (TryClosePolygon(Coordinates, MouseDownLocation.Value, mouseUpLocation))
                            break;
                       
                        // If mouse actions were a drag, drage the appropriate coord
                        if (TryDragCoord(Coordinates, MouseDownLocation.Value, mouseUpLocation))
                            break;

                        // Add a new point
                        if (PolygonClosed)
                        {
                            Coordinates.Clear();
                        }
                        Coordinates.Add(mouseUpLocation);
                    }
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    {
                        // Delete coord
                        try
                        {
                            var existing = Coordinates.First(c => mouseUpLocation.DistanceTo(c) <= CIRCLE_RADIUS);
                            var existingIndex = Coordinates.IndexOf(existing);
                            Coordinates.RemoveAt(existingIndex);
                        }
                        catch { }
                    }
                    break;
                default:
                    break;
            }

            // Make sure the mouse down location isn't re-used later accidentally
            MouseDownLocation = null;

            DrawCoords(Coordinates);
        }

        private void pbBgImage_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownLocation = new Point(e.Location.X, e.Location.Y);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Coordinates.Clear();
            DrawCoords(Coordinates);
        }

        public Image<Bgr, float> GetRoiMask()
        {
            var coords = new List<Point>();

            // Can't simply use fillConvexPoly here as we expect that the poly is not
            // usually convex.  Instead, draw the poly and use the flood tool to fill in the rest of the image with black

            // In order for flood tool not to mess up if roi extends from one end to another, move any points on the border inwards
            // by one pixel.  This allows a buffer for the flood tool to travel around.
            foreach (var coord in Coordinates)
            {
                var x = coord.X;
                var y = coord.Y;

                if (x <= 0) x = 1;
                if (y <= 0) y = 1;
                if (x >= pbBgImage.Image.Width) x = pbBgImage.Image.Width - 1;
                if (y >= pbBgImage.Image.Height) y = pbBgImage.Image.Height - 1;

                coords.Add(new Point(x, y));
            }

            var image = new Image<Bgr, float>(StartImage.Size);
            image.SetValue(new Bgr(Color.White));
            LineSegment2D l = new LineSegment2D();

            image.DrawPolyline(coords.ToArray(), true, new Bgr(Color.Black), 1);

            MCvConnectedComp comp = new MCvConnectedComp();

            MCvScalar lo = new MCvScalar(1, 1, 1);
            MCvScalar up = new MCvScalar(1, 1, 1);

            // Know it is safe to fill at 0,0 because we created a 1 pixel buffer around all coordinates
            CvInvoke.cvFloodFill(image.Ptr,
                new Point(0,0),
                new MCvScalar(0, 0, 0),
                lo,
                up,
                out comp,
                Emgu.CV.CvEnum.CONNECTIVITY.FOUR_CONNECTED,
                Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
                new IntPtr());

          //  image.Save("roi.bmp");
            return image;
        }
    }
}
