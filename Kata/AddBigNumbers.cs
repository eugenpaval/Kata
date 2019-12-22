using System;
using System.Text;

namespace Kata
{
    // 4 kyu
    public class AddBigNumbers
    {
        public static string Add(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a))
                return b;

            if (string.IsNullOrWhiteSpace(b))
                return a;

            var (pa, pb) = (a.Length - 1, b.Length - 1);
            var (result, carry) = (0, 0);
            var maxDigits = Math.Max(pa + 1 , pb + 1);
            var buffer = new char[maxDigits + 1];

            while (maxDigits >= 0)
            {
                var ca = pa >= 0 ? a[pa--] - '0' :  0;
                var cb = pb >= 0 ? b[pb--] - '0' : 0;

                (result, carry) = SimpleAddResult(ca, cb + carry);
                buffer[maxDigits] = (char) (result + 48);

                --maxDigits;
            }

            return new string(buffer).TrimStart('0');
        }

        public static (int, int) SimpleAddResult(int x, int y)
        {
            var r = x + y;
            return (r % 10,  r / 10);
        }
    }
}
