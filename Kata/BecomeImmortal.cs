using System;
using System.Numerics;

namespace Kata
{
    public static class Immortal
    {
        /// set true to enable debug
        public static bool Debug = false;

        public static long ElderAge(long n, long m, long k, long newp)
        {
            return Helper(new BigInteger(n), new BigInteger(m), new BigInteger(k), newp);
        }

        public static long Helper(BigInteger n, BigInteger m, BigInteger k, long newp)
        {
            Func<BigInteger, BigInteger> p2 = x =>
            {
                var result = 1L;
                while (result < x)
                    result <<= 1;

                return result;
            };

            Func<BigInteger, BigInteger, BigInteger> sumInterval = (lowerB, upperB) => (lowerB + upperB) * (upperB - lowerB + 1) / 2;
            Func<BigInteger, BigInteger, BigInteger> max = (x, y) => x >= y ? x : y;

            if (n == 0 || m == 0)
                return 0;

            if (n > m)
                (n, m) = (m, n);

            var pn = p2(n);
            var pm = p2(m);

            if (k > pm)
                return 0;

            BigInteger life = 0;
            if (pn == pm)
                life = sumInterval(1, pm - k - 1) * (n + m - pm) + Helper(pm - m, pn - n, k, newp);

            if (pn < pm)
            {
                pn = pm / 2;
                life = n * sumInterval(1, pm - k - 1) - (pm - m) * sumInterval(max(0, pn - k), pm - k - 1);

                if (k <= pn)
                    life += (pn - k) * (pn - n) * (pm - m) + Helper(pn - n, pm - m, 0, newp);
                else
                    life += Helper(pn - n, pm - m, k - pn, newp);
            }

            var result = (long)(life % newp);
            return result < 0 ? newp + result : result;
        }
    }
}