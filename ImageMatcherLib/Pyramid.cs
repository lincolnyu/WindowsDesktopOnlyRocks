using System.Drawing;

namespace ImageMatcherLib
{
    public class Pyramid
    {
        public Pyramid(Size[] block_sizes, HistoMap hmap)
        {
            _block_sizes = block_sizes;
            _histomap = hmap;
            _plane_sizes = new Size[_block_sizes.Length];
            _data = new byte[block_sizes.Length][,];
            for (var i = 0; i < block_sizes.Length; i++)
            {
                var block_size = block_sizes[i];
                var plane_width = _histomap.Width - block_size.Width + 1;
                var plane_height = _histomap.Height - block_size.Height + 1;
                _plane_sizes[i] = new Size(plane_width, plane_height);
                _data[i] = new byte[plane_height, plane_width];
            }
        }

        public byte this[int level, int x, int y]
        {
            get => _data[level][y, x];
            set { _data[level][y, x] = value; }
        }

        byte[][,] _data;
        Size[] _block_sizes;    // size of blocks
        Size[] _plane_sizes;
        HistoMap _histomap;
    }
}
