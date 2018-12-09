using System.Drawing;

namespace ImageMatcherLib
{
    public static class ImageHelper
    {
        public static double RgbToY(Color clr)
        {
            return 16 + (65.481 * clr.R + 128.533 * clr.G + 24.966 * clr.B) / 255;
        }

        public static uint ModAdd(uint a, uint b)
            => (a <= uint.MaxValue - b) ? a + b : a - (uint.MaxValue - b) - 1;

        public static uint ModSub(uint a, uint b)
            => (a > b) ? a - b : b - a;
    }
}
