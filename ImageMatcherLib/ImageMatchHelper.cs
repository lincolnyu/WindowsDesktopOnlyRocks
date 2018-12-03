using System;
using System.Drawing;

namespace handywin
{
    public static class ImageMatchHelper
    {
        static double RgbToY(Color clr)
        {
            return 16 + (65.481 * clr.R + 128.533 * clr.G + 24.966 * clr.B) / 255;
        }

        public static bool SearchAlmostExactMatch(this Bitmap ss, Bitmap target,
             out Point point)
            => SearchAlmostExactMatch(ss, target, 3 * 3, out point);

        // TODO optimize...
        public static bool SearchAlmostExactMatch(this Bitmap ss, Bitmap target,
             double mseThr, out Point point)
        {
            var srcWidth = ss.Width;
            var srcHeight = ss.Height;
            var tgtWidth = target.Width;
            var tgtHeight = target.Height;
            var tgtQW = tgtWidth / 4;
            var tgtQH = tgtHeight / 4;
            var tgtQ3W = tgtWidth * 3 / 4;
            var tgtQ3H = tgtHeight * 3 / 4;
            var tgtHW = tgtWidth / 2;
            var tgtHH = tgtHeight / 2;
            var tgtArea = tgtWidth * tgtHeight;

            for (var i = 0; i < srcWidth - tgtWidth; i++)
            {
                for (var j = 0; j < srcHeight - tgtHeight; j++)
                {
                    double mse = 0;
                    double thr = 0;
                    bool matching = true;
                    for (var x = 0; x < tgtWidth && matching; x++)
                    {
                        for (var y = 0; y < tgtHeight && matching; y++)
                        {
                            var yss = RgbToY(ss.GetPixel(i + x, j + y));
                            var ytgt = RgbToY(target.GetPixel(x, y));
                            var d = yss - ytgt;
                            mse += d * d;
                            thr += mseThr;
                            if (mse > thr)
                            {
                                matching = false;
                            }
                        }
                    }
                    if (!matching)
                    {
                        continue;
                    }
                    mse /= tgtArea;
                    if (mse < mseThr)
                    {
                        point = new Point(i, j);
                        return true;
                    }
                }
            }
            point = default(Point);
            return false;
        }

        private class GreyImage
        {
            public GreyImage(Bitmap bmp, int xpadding = 1, int ypadding = 1)
            {
                Width = bmp.Width;
                Height = bmp.Height;
                _data = new byte[bmp.Height + ypadding, bmp.Width + xpadding];
                for (var i = 0; i < bmp.Height; i++)
                {
                    for (var j = 0; j < bmp.Width; j++)
                    {
                        _data[i, j] = (byte)
                             Math.Min(Math.Round(RgbToY(bmp.GetPixel(j, i))), 255);
                    }
                    for (var j = bmp.Width; j < bmp.Width + xpadding; j++)
                    {
                        _data[i, j] = _data[i, bmp.Width - 1];
                    }
                }
                for (var i = bmp.Height; i < bmp.Height + ypadding; i++)
                {
                    for (var j = 0; j < bmp.Width + xpadding; j++)
                    {
                        _data[i, j] = _data[bmp.Height - 1, j];
                    }
                }
            }

            public byte this[int x, int y] => _data[y, x];
            public int Height { get; }
            public int Width { get; }

            byte[,] _data;
        }

        static uint ModAdd(uint a, uint b)
            => (a <= uint.MaxValue - b) ? a + b : a - (uint.MaxValue - b) - 1;

        static uint ModSub(uint a, uint b)
            => (a > b) ? a - b : b - a;

        class HistoMap
        {
            public HistoMap(GreyImage gi)
            {
                _data = new uint[gi.Height + 1, gi.Width + 1];
                for (var i = 1; i < gi.Height; i++)
                {
                    for (var j = 1; j < gi.Width; j++)
                    {
                        _data[i, j] = ModAdd(ModSub(ModAdd(_data[i, j - 1], _data[i - 1, j]), _data[i - 1, j - 1]), gi[i - 1, j - 1]);
                    }
                }
            }

            public float GetHisto(Rectangle rect)
            {
                var sum = (float)GetSum(rect);
                return sum / (rect.Width * rect.Height);
            }

            public uint GetSum(Rectangle rect)
            {
                var b = _data[rect.Y, rect.X];
                var bx = _data[rect.Y, rect.X + rect.Width];
                var by = _data[rect.Y + rect.Height, rect.X];
                var bxy = _data[rect.Y + rect.Height, rect.X + rect.Width];
                return ModSub(ModAdd(bxy, b), ModAdd(bx, by));
            }

            uint[,] _data;
        }

        public static bool Search(this Bitmap src, Bitmap blk, ref double mseLow, out Point location)
        {
            var giSrc = new GreyImage(src);
            var giBlk = new GreyImage(blk);
            return Search(giSrc, giBlk, ref mseLow, out location);
        }

        static bool Search(this Bitmap src, Bitmap blk, out Point location)
        {
            var giSrc = new GreyImage(src);
            var giBlk = new GreyImage(blk);
            return Search(giSrc, giBlk, out location);
        }

        private static bool SearchGreyPyramid(GreyImage src, GreyImage blk,
            ref double mseLow, out Point location)
        {
            throw new NotImplementedException();
        }

        private static void Refine(GreyImage src, GreyImage blk,
            ref Point point, ref double r, out double mse, double vibr = 0.2)
        {
            int steph = (int)Math.Round(blk.Width * r * vibr);
            int stepv = (int)Math.Round(blk.Height * r * vibr);
            mse = double.MaxValue;
            Refine(src, ref point, blk, r, steph, stepv, ref mse);
            Refine(src, ref point, blk, ref r, ref mse, 0.5, 2.0);
        }

        static void Refine(GreyImage src, ref Point location, GreyImage blk,
            ref double r, ref double mseLow,
            double rmin, double rmax, double rr = 1.05)
        {
            Point orloc = location;
            double orir = r;
            for (var ir = orir * rr; ir < rmax; ir *= rr)
            {
                var loc = new Point(
                    (int)Math.Round(orloc.X - blk.Width * (ir - orir) / 2),
                    (int)Math.Round(orloc.Y - blk.Height * (ir - orir) / 2));
                if (MseLower(src, loc.X, loc.Y, blk, ir, ref mseLow))
                {
                    location = loc;
                    r = ir;
                }
                else
                {
                    break;
                }
            }
            for (var ir = orir / rr; ir > rmin; ir /= rr)
            {
                var loc = new Point(
                    (int)Math.Round(orloc.X - blk.Width * (ir - orir) / 2),
                    (int)Math.Round(orloc.Y - blk.Height * (ir - orir) / 2));
                if (MseLower(src, loc.X, loc.Y, blk, ir, ref mseLow))
                {
                    location = loc;
                    r = ir;
                }
                else
                {
                    break;
                }
            }
        }

        static void Refine(GreyImage src, ref Point location,
            GreyImage blk, double r, int step_h, int step_v, ref double mseLow)
        {
            var rsize_v = r * src.Height;
            var rsize_h = r * src.Width;
            bool updated = false;
            Point newLoc = default(Point);
            if (location.X >= step_h && location.Y >= step_v &&
                MseLower(src, location.X - step_h, location.Y - step_v, blk,
                r, ref mseLow))
            {
                updated = true;
                newLoc = new Point(location.X - step_h, location.Y - step_v);
            }
            if (location.X >= step_h && location.Y + step_v + rsize_v < src.Height &&
                MseLower(src, location.X - step_h, location.Y + step_v, blk,
                r, ref mseLow))
            {
                updated = true;
                newLoc = new Point(location.X - step_h, location.Y + step_v);
            }
            if (location.X + step_h + rsize_h < src.Width &&
                location.Y >= step_v &&
                MseLower(src, location.X + step_h, location.Y - step_v, blk,
                r, ref mseLow))
            {
                updated = true;
                newLoc = new Point(location.X + step_h, location.Y - step_v);
            }
            if (location.Y + step_v + rsize_v < src.Height &&
                location.X + step_h + rsize_h < src.Width &&
                MseLower(src, location.X + step_h, location.Y + step_v, blk,
                r, ref mseLow))
            {
                updated = true;
                newLoc = new Point(location.X + step_h, location.Y + step_v);
            }

            if (updated)
            {
                location = newLoc;
            }

            step_h /= 2;
            step_v /= 2;

            if (step_h > 0 && step_v > 0)
            {
                Refine(src, ref location, blk, r, step_h, step_v, ref mseLow);
            }
        }

        static bool Search(GreyImage src, GreyImage blk, out Point location)
        {
            var mseLow = double.MaxValue;
            return Search(src, blk, ref mseLow, out location);
        }

        const double stepRatio = 0.125;

        static bool Search(GreyImage src, GreyImage blk, ref double mseLow,
            out Point location)
        {
            var rs = new double[] { 1.0, 1.25, 1.6, 2.0 };
            bool found = false;
            double found_r = 0;
            location = default(Point);
            foreach (var r in rs)
            {
                if (Search(src, blk, r, ref mseLow, ref location))
                {
                    found = true;
                    found_r = r;
                }
            }
            if (!found)
            {
                return false;
            }
            var step_h = (int)Math.Round(found_r * blk.Width * stepRatio / 4);
            var step_v = (int)Math.Round(found_r * blk.Height * stepRatio / 4);

            if (step_h > 0 && step_v > 0)
            {
                Refine(src, ref location, blk, found_r, step_h, step_v, ref mseLow);
            }
            return true;
        }

        static bool Search(GreyImage src, GreyImage blk, double r,
            ref double mseLow, ref Point location)
        {
            var result = false;
            var rsize_h = blk.Width * r;
            var rsize_v = blk.Height * r;
            var rstep_h = (int)Math.Round(rsize_h * stepRatio);
            var rstep_v = (int)Math.Round(rsize_v * stepRatio);
            for (var i = 0; i + rsize_v < src.Height; i += rstep_v)
            {
                for (var j = 0; j + rsize_h < src.Width; j += rstep_h)
                {
                    var mse = mseLow;
                    if (MseLower(src, j, i, blk, r, ref mse))
                    {
                        mseLow = mse;
                        location = new Point(j, i);
                        result = true;
                    }
                }
            }
            return result;
        }

        static bool MseLower(GreyImage src, int x, int y, GreyImage blk,
            double r, ref double mseLow)
        {
            double mse = 0;
            var area = blk.Height * blk.Width;
            var thr = mseLow * area;
            // bilinear
            for (var i = 0; i < blk.Height; i++)
            {
                for (var j = 0; j < blk.Width; j++)
                {
                    var pblk = blk[j, i];
                    // r neighbouring pixels but r <= 2
                    var xr = j * r;
                    var yr = i * r;

                    var xr_f = (int)Math.Floor(xr);
                    var yr_f = (int)Math.Floor(yr);

                    var xw_c = xr - xr_f;
                    var yw_c = yr - yr_f;
                    var xw_f = 1 - xw_c;
                    var yw_f = 1 - yw_c;

                    // assuming it's padded
                    var psrc = src[x + xr_f, y + yr_f] * xw_f * yw_f
                    + src[x + xr_f + 1, y + yr_f] * xw_c * yw_f
                    + src[x + xr_f, y + yr_f + 1] * xw_f * yw_c
                    + src[x + xr_f + 1, y + yr_f + 1] * xw_c * yw_c;

                    var diff = psrc - pblk;
                    diff *= diff;
                    mse += diff;
                    // TODO ...
                    if (mse >= thr) return false;
                }
            }
            mseLow = mse / area;
            return true;
        }
    }
}
