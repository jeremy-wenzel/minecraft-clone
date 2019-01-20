using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Perlin
    {
        private const int _xMax = 256;
        private const int _yMax = 256;
        private float[, ,] _gradient;

        public Perlin()
        {
            _gradient = new float[_xMax, _yMax, 2];
            GeneratePerlin();
        }

        private void GeneratePerlin()
        {
            Random random = new Random();
            for (int i = 0; i < _xMax; ++i)
            {
                for (int j = 0; j < _yMax; ++j)
                {
                    _gradient[i, j, 0] = random.Next() / (int.MaxValue / 2) - 1.0f;
                    _gradient[i, j, 1] = random.Next() / (int.MaxValue / 2) - 1.0f;
                }
            }
        }

        private float Lerp(float a0, float a1, float w)
        {
            return (1.0f - w) * a0 + w * a1;
        }

        private float DotGridGradient(int ix, int iy, float x, float y)
        {
            float dx = x - (float)ix;
            float dy = y - (float)iy;
            return (dx * _gradient[iy, ix, 0] + dy * _gradient[iy, ix, 1]);
        }

        public float DoPerlin(float x, float y)
        {
            int x0 = (int)x;
            int x1 = x0 + 1;
            int y0 = (int)y;
            int y1 = y0 + 1;

            float sx = x - (float)x0;
            float sy = y - (float)y0;

            float n0, n1, ix0, ix1, value;
            n0 = DotGridGradient(x0, y0, x, y);
            n1 = DotGridGradient(x1, y0, x, y);
            ix0 = Lerp(n0, n1, sx);
            n0 = DotGridGradient(x0, y1, x, y);
            n1 = DotGridGradient(x1, y1, x, y);
            ix1 = Lerp(n0, n1, sx);
            value = Lerp(ix0, ix1, sy);

            return value;
        }
    }
}
