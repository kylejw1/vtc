using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace TemplateMatcher
{
    public class TemplateMatcher
    {
        public Image<Bgra, byte>[] Templates; // Array of template images 

        public TemplateAssignment[] CalculateBestAssignment(Image<Bgr, byte> frame)
        {
            return new TemplateAssignment[0];
        }

        public TemplateAssignment SingleBestAssignment(Image<Bgra, byte> image)
        {
            var templateAssignments = new TemplateAssignment[1];
            templateAssignments[0] = new TemplateAssignment();
            templateAssignments[0].XShift = 0;
            templateAssignments[0].YShift = 0;
            templateAssignments[0].Template = 0;
            var bestTemplateAssignment = new TemplateAssignment();
            bestTemplateAssignment.XShift = 0;
            bestTemplateAssignment.YShift = 0;
            bestTemplateAssignment.Template = 0;

            double bestAssignmentValue = EvaluateAssignment(templateAssignments, image);

            for(int k=0; k<Templates.Length;k++)
            for(int i=0; i<image.Width;i+=3)
                for (int j = 0; j < image.Height; j+=3)
                {
                    templateAssignments[0].Template = k;
                    templateAssignments[0].XShift = i;
                    templateAssignments[0].YShift = j;
                    var assignmentValue = EvaluateAssignment(templateAssignments, image);

                    if (assignmentValue < bestAssignmentValue)
                    {
                        bestAssignmentValue = assignmentValue;
                        bestTemplateAssignment.XShift = i;
                        bestTemplateAssignment.YShift = j;
                        bestTemplateAssignment.Template = k;
                    }
                }

            return bestTemplateAssignment;
        }

        public void PopulateTemplates(string templatePath)
        {
            var paths = Directory.GetFiles(templatePath);
            Templates = new Image<Bgra, byte>[paths.Length];
            for (var i = 0; i < paths.Length; i++)
                Templates[i] = new Image<Bgra, byte>(paths[i]);
        }

        public double EvaluateAssignment(TemplateAssignment[] candidate, Image<Bgra, byte> input)
        {
            var image = ReconstructImage(candidate, input.Width, input.Height);
            var differenceImage = input.AbsDiff(image);
            var reconstructionErrorSum = differenceImage.GetSum().Red + differenceImage.GetSum().Green + differenceImage.GetSum().Blue;
            return reconstructionErrorSum;
        }

        public TemplateAssignment[][] EnumerateAssignments(Image<Bgr, byte> frame)
        {
            return new TemplateAssignment[0][];
        }

        public Image<Bgra, byte> ReconstructImage(TemplateAssignment[] assignment, int width, int height)
        {
            var image = new Image<Bgra, byte>(width, height, new Bgra(0, 0, 0, 0));
            var paddedImage = PaddedImage(image);
            var maxDimension = Templates.Select(template => Math.Max(template.Width, template.Height)).Concat(new[] { 0 }).Max();

            var bmp = paddedImage.Bitmap;
            var gra = Graphics.FromImage(bmp);
            gra.CompositingMode = CompositingMode.SourceOver;
            for (int i = 0; i < assignment.Length; i++)
            {
                var template = Templates[assignment[i].Template];
                int x = assignment[i].XShift;
                int y = assignment[i].YShift;
                paddedImage.ROI = new Rectangle(x, y, template.Width, template.Height);
                gra.DrawImage(template.ToBitmap(), new Point(x, y));
            }

            paddedImage.ROI = new Rectangle(maxDimension, maxDimension, image.Width, image.Height);
            paddedImage.CopyTo(image);
            return image;
        }

        public Image<Bgra, byte> PaddedImage(Image<Bgra, byte> imageIn)
        {
            var maxDimension = Templates.Select(template => Math.Max(template.Width, template.Height)).Concat(new[] { 0 }).Max();
            var paddedIm = new Image<Bgra, byte>(new Size(imageIn.Width + 2 * maxDimension, imageIn.Height + 2 * maxDimension));
            paddedIm.ROI = new Rectangle(maxDimension, maxDimension, imageIn.Width, imageIn.Height);
            imageIn.CopyTo(paddedIm);
            return paddedIm;
        }

    }

    public struct TemplateAssignment
    {
        public int Template;
        public int XShift;
        public int YShift;
    }
}
