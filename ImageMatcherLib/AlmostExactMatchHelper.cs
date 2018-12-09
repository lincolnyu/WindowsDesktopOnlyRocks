using System.Drawing;

namespace ImageMatcherLib
{
    using static ImageHelper;

    public static class AlmostExactMatchHelper
    {
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
    }
}
