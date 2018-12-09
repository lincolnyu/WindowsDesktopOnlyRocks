using System;
using System.Drawing;

namespace ImageMatcherLib
{
    using static ImageHelper;

    public class GreyImage
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
                    _data[i, j] = (byte)Math.Min(Math.Round(RgbToY(bmp.GetPixel(j, i))), 255);
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
}
