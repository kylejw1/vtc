﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using VTC.Kernel.Extensions;
using VTC.Kernel.RegionConfig;
using NetTopologySuite;

namespace VTC
{
    public partial class PolygonBuilderControl : UserControl
    {
        public event EventHandler OnDoneClicked;
        public event EventHandler OnCancelClicked;

        private Image<Bgr, float> StartImage;
        private Point? MouseDownLocation = null;

        public Polygon Coordinates = new Polygon();
        private Polygon InitialCoordinates;

        public int CircleRadius = 5;
        public int LineDrawWidth = 2;

        public Bgr IntersectionColor = new Bgr(Color.Red);
        public Bgr IncompleteColor = new Bgr(Color.Blue);
        public Bgr CompleteColor = new Bgr(Color.Green);
        public Bgr CircleColor = new Bgr(Color.Green);

        public PolygonBuilderControl(Image<Bgr, float> _bgImage, Polygon startCoords)
        {
            InitializeComponent();
            
            InitialCoordinates = startCoords;

            StartImage = _bgImage.Clone();

            pictureBox1.Image = StartImage.ToBitmap();

            pictureBox1.Size = StartImage.Size;

            if (null != startCoords)
            {
                foreach (var coord in startCoords)
                {
                    Coordinates.Add(coord);
                }
            }

            Draw(StartImage);
        }

        private void pbBgImage_MouseUp(object sender, MouseEventArgs e)
        {
            // Discard errant mouseups
            if (null == MouseDownLocation) return;

            var mouseUpLocation = new Point(e.Location.X, e.Location.Y);

            if (mouseUpLocation.X >= StartImage.Width) mouseUpLocation.X = StartImage.Width - 1;
            if (mouseUpLocation.Y >= StartImage.Height) mouseUpLocation.Y = StartImage.Height - 1;

            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    {
                        // If clicked on first existing point, close polygon
                        if (TryClosePolygon((Point)MouseDownLocation, mouseUpLocation))
                            break;

                        // If start and end points are far apart, drag the appropriate coord
                        if (TryDragCoord((Point)MouseDownLocation, mouseUpLocation))
                            break;

                        // If nothing else, insert a new coordinate

                        // If the polygon is already closed, start over 
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
                            var existing = Coordinates.First(c => mouseUpLocation.DistanceTo(c) <= CircleRadius);
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

            Draw(StartImage);
        }

        private void pbBgImage_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownLocation = new Point(e.Location.X, e.Location.Y);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Coordinates.Clear();

            Draw(StartImage);
        }

        public void Draw(Image<Bgr, float> source)
        {
            var newImage = source.Clone();

            var segments = GetSegments();
            var intersectingSegments = segments.Where(s => LineSegment2D_IntersectsAny(s, segments)).ToList();
            segments.RemoveAll(s => intersectingSegments.Contains(s));

            // If closed, draw green.  Otherwise, blue.  Intersections are red
            var segmentColor = PolygonClosed ? CompleteColor : IncompleteColor;

            // Draw good line segments
            foreach (var segment in segments)
            {
                newImage.Draw(segment, segmentColor, LineDrawWidth);
            }

            // Draw intersecting segments
            foreach (var intersection in intersectingSegments)
            {
                newImage.Draw(intersection, IntersectionColor, LineDrawWidth);
            }

            // Mark verticies with circle
            foreach (var point in Coordinates)
            {
                newImage.Draw(new CircleF(point, CircleRadius), CircleColor, LineDrawWidth);
            }

            pictureBox1.Image = newImage.ToBitmap();

            if (Coordinates.Count() == 0)
            {
                tbMessages.Text = "Click OK to accept changes.";
                btnDone.Enabled = true;
            } 
            else if (intersectingSegments.Count() > 0)
            {
                tbMessages.Text = "Polygon can not contain intersecting segments";
                btnDone.Enabled = false;
            }
            else if (!PolygonClosed)
            {
                tbMessages.Text = "Polygon is not closed";
                btnDone.Enabled = false;
            }
            else
            {
                tbMessages.Text = "Click OK to accept changes.";
                btnDone.Enabled = true;
            }
        }

        private bool PolygonClosed
        {
            get
            {
                if (Coordinates.Count() < 3) return false;

                return (Coordinates.First().Equals(Coordinates.Last()));
            }
        }

        private List<LineSegment2D> GetSegments()
        {
            var lines = new List<LineSegment2D>();

            if (Coordinates.Count() < 2) return lines;

            for (int i = 1; i < Coordinates.Count(); i++)
            {
                lines.Add(new LineSegment2D(Coordinates.ElementAt(i - 1), Coordinates.ElementAt(i)));
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

        private bool TryClosePolygon(Point down, Point up)
        {
            // Close the polygon if the down and up points are close (meaning nothing was dragged), 
            // and they are close to the first coordinate in the list

            if (PolygonClosed) return false;

            if (null == Coordinates || Coordinates.Count() < 3) return false;

            if (down.DistanceTo(up) > CircleRadius) return false;

            if (up.DistanceTo(Coordinates.First()) > CircleRadius) return false;

            // Close the polygon
            Coordinates.Add(Coordinates.First());

            //Calculate centroid
            //GeoAPI.Geometries.Coordinate[] points = new GeoAPI.Geometries.Coordinate[Coordinates.Count];
            var points = Coordinates.Select(x => new GeoAPI.Geometries.Coordinate(x.X, x.Y)).ToArray();
            NetTopologySuite.Geometries.LinearRing ring = new NetTopologySuite.Geometries.LinearRing(points);
            NetTopologySuite.Geometries.Polygon ntsPoly = new NetTopologySuite.Geometries.Polygon(ring);
            NetTopologySuite.Algorithm.Centroid ntsCentroid = new NetTopologySuite.Algorithm.Centroid(ntsPoly);
            Coordinates.Centroid.X = (int) ntsCentroid.GetCentroid().X;
            Coordinates.Centroid.Y = (int) ntsCentroid.GetCentroid().Y;

            return true;
        }

        private bool TryDragCoord(Point start, Point end)
        {
            int indexOfExisting;
            Point coord;

            try
            {
                coord = Coordinates.First(c => start.DistanceTo(c) <= CircleRadius);
                indexOfExisting = Coordinates.IndexOf(coord);
            }
            catch
            {
                return false;
            }

            if (end.X < 0) end.X = 0;
            //if (end.X >= Width) end.X = Width - 1;
            if (end.Y < 0) end.Y = 0;
            //if (end.Y >= Height) end.Y = Height - 1;

            coord.X = end.X;
            coord.Y = end.Y;
            Coordinates.RemoveAt(indexOfExisting);
            Coordinates.Insert(indexOfExisting, coord);

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (null != OnCancelClicked)
            {
                OnCancelClicked(sender, e);
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (null != OnDoneClicked)
            {
                OnDoneClicked(sender, e);
            }
        }
    }
}
