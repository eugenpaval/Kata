using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Kata
{
    public class BinomialExpansion
    {
        public static string Expand(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
                return "";

            var regex = new Regex(@"\(\s*([+,-]*\d*)\s*(\w)\s*([+,-]\s*\d+)\s*\)\s*\^\s*(\d+)");

            var mc = regex.Matches(expr);
            if (mc.Count == 0)
                throw new Exception("Input expression is not given in the stated form");

            var (a, x, b, p) = (mc[0].Groups[1].Value, mc[0].Groups[2].Value, mc[0].Groups[3].Value, mc[0].Groups[4].Value);
            
            if (a == "")
                a = "1";
            else if (a == "-")
                a = "-1";

            return new BinomialExpansion().Expand(int.Parse(a), x, int.Parse(b), int.Parse(p));
        }

        private string Expand(int a, string x, int b, int p)
        {
            if (p == 0)
                return "1";

            var sb = new StringBuilder();

            foreach (var repr in GetTokens(a, b, p).Select(t => Format(t.term, t.power, x)))
            {
                if (repr.Length > 0 && repr[0] != '-')
                    sb.Append("+");
                sb.Append(repr);
            }

            return sb[0] == '+' ? sb.ToString().Substring(1) : sb.ToString();
        }

        private BigInteger Combinations(int n, int m)
        {
            var result = new BigInteger(1);
            for (var i = 0; i < m; ++i)
                result *= n - i;

            return result / Factorial(m);
        }

        private BigInteger Factorial(int n)
        {
            var result = new BigInteger(1);
            while (n > 1) 
                result *= n--;

            return result;
        }

        private BigInteger Power(int n, int p)
        {
            var result = new BigInteger(n);
            while (p-- > 1)
                result *= n;

            return p == -1 ? 1 : result;
        }

        private IEnumerable<(BigInteger term, int power)> GetTokens(int a, int b, int p)
        {
            for (var i = 0; i <= p; ++i)
            {
                var c = Combinations(p, i);
                var npart = Power(a, p - i) * Power(b, i);
                var term = c * npart;

                yield return (term, p-i);
            }
        }

        private string Format(BigInteger term, int exp, string x)
        {
            if (term == 0)
                return "";

            if (exp == 0)
                return $"{term}";

            var expS = exp == 1 ? "" : $"^{exp}";

            if (term == -1)
                return $"-{x}{expS}";

            if (term == 1)
                return $"{x}{expS}";

            return $"{term}{x}{expS}";
        }
    }
}
