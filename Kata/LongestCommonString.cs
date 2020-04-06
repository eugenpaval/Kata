using System;
using System.Text;

namespace Kata
{
    public class LcsClass
    {
        public static string Lcs(string a, string b)
        {
            var lengths = new int[a.Length + 1, b.Length + 1];

            for (var i = 0; i <= a.Length; i++)
            {
                for (var j = 0; j <= b.Length; j++)
                {
                    if (i == 0 || j == 0)
                        lengths[i, j] = 0;
                    else if (a[i - 1] == b[j - 1])
                        lengths[i, j] = lengths[i - 1, j - 1] + 1;
                    else
                        lengths[i, j] = Math.Max(lengths[i - 1, j], lengths[i, j - 1]);
                }
            }

            var index = lengths[a.Length, b.Length];
            var lcs = new StringBuilder(index);
            var k = a.Length;
            var l = b.Length;

            while (k > 0 && l > 0)
            {
                if (a[k - 1] == b[l - 1])
                {
                    lcs.Insert(0, a[k - 1]);
                    --k; --l; --index;
                }
                else if (lengths[k - 1, l] > lengths[k, l - 1])
                    --k;
                else
                    --l;
            }

            return lcs.ToString();
        }
    }
}
