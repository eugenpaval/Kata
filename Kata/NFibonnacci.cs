using System;
using System.Numerics;

namespace Kata
{
    public class NFibonnacci
    {
        // this method doesn't work for large numbers due to loss of precision in Math.Pow
        //public static BigInteger fib(int n)
        //{
        //    var sqrt5 = Math.Sqrt(5);
        //    var w = (1 + sqrt5) / 2;
        //    var retVal = new BigInteger(Math.Pow(w, Math.Abs(n)) / sqrt5);

        //    return n > 0 || n % 2 == 1 ? retVal : -retVal;
        //}

        // this method uses the fact that we can represent the calculation of a fibonacci number as 
        // a matrix transformation so that
        //
        //          | F(n-1)   F(n) |   | 0  1 |     
        // F(n+2) = |               | * |      | 
        //          | F(n)   F(n+1) |   | 1  1 |
        //      
        // Pow(A, n) = n % 2 == 0 ? Pow(A, n/2) * Pow(A, n/2) ? A * Pow(A, n-1)
        //
        public static BigInteger fib(int n)
        {
            if (n == 0)
                return 0;

            var retVal = PositiveFib(n > 0 ? n : -n);
            return n < 0 && n % 2 == 0 ? -retVal : retVal;
        }

        private static readonly BigInteger[] _preCalc = new BigInteger[200000];

        private static BigInteger PositiveFib(int n)
        {
            if (n == 1 || n == 2 || n == -1)
                return 1;

            if (_preCalc[n] != 0)
                return _preCalc[n];

            Func<int, BigInteger> a = x => PositiveFib(x - 1);
            Func<int, BigInteger> b = PositiveFib;
            Func<BigInteger, BigInteger, BigInteger> compute;

            int k;

            if (n % 2 == 0)
            {
                k = n / 2;
                compute = (p, q) => q * (2 * p + q);
            }
            else
            {
                k = (n + 1) / 2;
                compute = (p, q) => p * p + q * q;
            }

            var retVal = compute(a(k), b(k));
            _preCalc[n] = retVal;

            return retVal;
        }
    }
}
