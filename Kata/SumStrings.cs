using System.Numerics;

namespace Kata
{
    public class SumStrings
    {
        public static string sumStrings(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a))
                return b;

            if (string.IsNullOrWhiteSpace(b))
                return a;

            return (BigInteger.Parse(a) + BigInteger.Parse(b)).ToString();
        }
    }
}
