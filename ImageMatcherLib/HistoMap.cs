using System.Drawing;

namespace ImageMatcherLib
{
    using static ImageHelper;

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
}
