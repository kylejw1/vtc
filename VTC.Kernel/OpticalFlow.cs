using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VTC.Kernel
{
    public class OpticalFlow
    {
        public static double SumSquareError(int[] target, int[] input)
        {
            if (target.Length != input.Length)
                throw new Exception("Can't compute error of inequal-length vectors.");

            double error = 0;

            var targetsAndInputs = target.ToList().Zip(input.ToList(), (x, y) => Math.Pow(x - y, 2));
            var sumSquareError = targetsAndInputs.Sum();
            return sumSquareError;
        }

        public static double SumSquareError(int[][] target, int[][] input)
        {
            if (target.Length != input.Length)
                throw new Exception("Can't compute error of inequal-length vectors.");

            var sseList = target.ToList().Zip(input.ToList(), SumSquareError);
            var sumSquareError = sseList.Sum();
            return sumSquareError;
        }

        public static double SumSquareError(int[][][] target, int[][][] input)
        {
            if (target.Length != input.Length)
                throw new Exception("Can't compute error of inequal-length vectors.");

            var sseList = target.ToList().Zip(input.ToList(), SumSquareError);
            var sumSquareError = sseList.Sum();
            return sumSquareError;
        }

        public static double SumSquareError(Image<Bgr, byte> im_target,Image<Bgr,byte> im_input, int x1, int y1, int x2, int y2, int width)
        {
            if(x1 + width/2 >= im_target.Width || x2 + width/2 >= im_target.Width || y1 + width/2 >= im_target.Height || y2 + width/2 >= im_target.Height)
                throw new Exception("Window out of range");

            if (x1 - width / 2 < 0 || x2 - width / 2 < 0 || y1 - width / 2 < 0 || y2 - width / 2 < 0)
                throw new Exception("Window out of range");

            double sse = 0;
            for (int i = -width/2; i < width/2; i++)
                for (int j = -width/2; j < width/2; j++)
                {
                    sse += Math.Pow(im_target.Data[y1 + j, x1 + i, 0] - im_input.Data[y2 + j, x2 + i, 0], 2);
                    sse += Math.Pow(im_target.Data[y1 + j, x1 + i, 1] - im_input.Data[y2 + j, x2 + i, 1], 2);
                    sse += Math.Pow(im_target.Data[y1 + j, x1 + i, 2] - im_input.Data[y2 + j, x2 + i, 2], 2);
                }

            return sse;
        }

        public static double SumSquareError(Image<Gray, byte> im_target, Image<Gray, byte> im_input,  int x1, int y1, int x2, int y2, int width)
        {
            if (x1 + width / 2 >= im_target.Width || x2 + width / 2 >= im_target.Width || y1 + width / 2 >= im_target.Height || y2 + width / 2 >= im_target.Height)
                throw new Exception("Window out of range");

            if (x1 - width / 2 < 0 || x2 - width / 2 < 0 || y1 - width / 2 < 0 || y2 - width / 2 < 0)
                throw new Exception("Window out of range");

            double sse = 0;
            for (int i = -width / 2; i < width / 2; i++)
                for (int j = -width / 2; j < width / 2; j++)
                    sse += Math.Pow(im_target.Data[y1 + j, x1 + i, 0] - im_input.Data[y2 + j, x2 + i, 0], 2);

            return sse;
        }

        public static List<OffsetSSEPair> OffsetSSEPairs(Image<Bgr, byte> im_t1, Image<Bgr, byte> im_t2, int x, int y, int resolution, int range, int aperture)
        {
            List<OffsetSSEPair> pairs = new List<OffsetSSEPair>();
            for(int i=-range; i<range; i += resolution)
                for (int j = -range; j < range; j += resolution)
                {
                    bool firstWindowInRange = x >= aperture && x < im_t1.Width - aperture &&
                                                 y >= aperture && y < im_t1.Height - aperture;

                    bool secondWindowInRange = x + i >= aperture && x + i < im_t1.Width - aperture &&
                                                 y + j >= aperture && y + j < im_t1.Height - aperture;
                    
                    if (firstWindowInRange && secondWindowInRange)
                    {
                        OffsetSSEPair osp = new OffsetSSEPair();
                        osp.XOffset = i;
                        osp.YOffset = j;
                        osp.SSE = SumSquareError(im_t1, im_t2, x, y, x + i, y + j, aperture);
                        pairs.Add(osp);    
                    }
                }

            return pairs;
        }

        public static List<OffsetSSEPair> OffsetSSEPairs(Image<Gray, byte> im_t1, Image<Gray, byte> im_t2, int x, int y, int resolution, int range, int aperture)
        {
            List<OffsetSSEPair> pairs = new List<OffsetSSEPair>();
            for (int i = -range; i < range; i += resolution)
                for (int j = -range; j < range; j += resolution)
                {
                    OffsetSSEPair osp = new OffsetSSEPair();
                    osp.XOffset = i;
                    osp.YOffset = j;
                    osp.SSE = SumSquareError(im_t1, im_t2, x, y, x + i, y + j, aperture);
                    pairs.Add(osp);
                }

            return pairs;
        }

        public static OffsetSSEPair LowestSSEPair(Image<Bgr, byte> im_t1, Image<Bgr, byte> im_t2, int x, int y, int resolution, int range, int aperture)
        {
            List<OffsetSSEPair> pairs = OffsetSSEPairs(im_t1, im_t2, x, y, resolution, range, aperture);
            List<OffsetSSEPair> sorted = pairs.OrderBy(o => o.SSE).ToList();
            return sorted.FirstOrDefault();
        }

        public static OffsetSSEPair LowestSSEPair(Image<Gray, byte> im_t1, Image<Gray, byte> im_t2, int x, int y, int resolution, int range, int aperture)
        {
            List<OffsetSSEPair> pairs = OffsetSSEPairs(im_t1, im_t2, x, y, resolution, range, aperture);
            List<OffsetSSEPair> sorted = pairs.OrderBy(o => o.SSE).ToList();
            return sorted.FirstOrDefault();
        }

        public struct OffsetSSEPair
        {
            public int XOffset;
            public int YOffset;
            public double SSE;
        } 

        

    }
}
