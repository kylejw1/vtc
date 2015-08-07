using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace TemplateMatcher
{
    public class TempateMatcher
    {
        public Image<Bgra, byte>[] Templates; // Array of template images 

        public TemplateAssignment[] CalculateBestAssignment(Image<Bgr, byte> frame)
        {
            return new TemplateAssignment[0];
        }

        public void PopulateTemplates(string templatePath)
        {
            
        }

        public double EvaluateAssignment(TemplateAssignment[] candidate)
        {
            return 0;
        }

        public TemplateAssignment[][] EnumerateAssignments(Image<Bgr, byte> frame)
        {
            return new TemplateAssignment[0][];
        }

        public Image<Bgr, byte> ReconstructImage(TemplateAssignment[] assignment)
        {
            return new Image<Bgr, byte>(1,1);
        }

    }

    public struct TemplateAssignment
    {
        public Image<Bgr, byte> Template;
        public int XShift;
        public int YShift;
    }
}
