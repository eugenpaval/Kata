using System;

namespace Kata
{
    public class PyramidSlide
    {
        public static int LongestSlideDown(int[][] pyramid)
        {
            if (pyramid.Length == 0)
                return 0;

            for (var level = pyramid.Length - 1; level > 0; --level)
            for (var i = 0; i < pyramid[level].Length - 1; ++i)
            {
                var value = Math.Max(pyramid[level][i] + pyramid[level - 1][i], pyramid[level][i + 1] + pyramid[level - 1][i]);
                pyramid[level - 1][i] = value;
            }

            return pyramid[0][0];
        }
    }
}
