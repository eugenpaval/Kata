using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class PrimeStreaming
    {
        public static IEnumerable<int> Stream()
        {
            return GetPrimes(int.MaxValue).Select(p => (int) p);
        }

        public static IEnumerable<long> GetPrimes(long x)
        {
            var limit = (long)Math.Floor(Math.Sqrt(x) + 1);
            var elemsInSieve = (limit >= int.MaxValue ? (int)limit / 2 : (int)limit) + 1;
            var primes = ErathosteneSieve(elemsInSieve);

            foreach (var p in primes)
                yield return p;

            var candidates = new BitArray(elemsInSieve + 1);
            var sieves = x / elemsInSieve + (x % elemsInSieve > 0 ? 1 : 0);

            for (var s = 1; s < sieves; ++s)
            {
                var start = (long)s * elemsInSieve;
                var stop = start + elemsInSieve > x ? x : start + elemsInSieve;

                candidates.SetAll(true);
                foreach (var p in primes)
                {
                    var from = (long)Math.Floor((decimal)start / p) * p;
                    if (from < start)
                        from += p;

                    for (var j = from; j < stop; j += p)
                        candidates[(int)(j - start)] = false;
                }

                for (var j = 0; j < elemsInSieve && j <= x - start; ++j)
                    if (candidates[j])
                        yield return j + start;
            }
        }

        // returns a bit array where a false value denotes a prime number
        private static IList<int> ErathosteneSieve(int upTo)
        {
            var primes = new BitArray(upTo + 1);
            primes.SetAll(true);

            for (var p = 2; p * p <= upTo; ++p)
                if (primes[p])
                    for (var i = p * 2; i <= upTo; i += p)
                        primes[i] = false;

            return primes.Cast<bool>().Select((e, index) => (e, index)).Where(v => v.index > 1 && v.e).Select(v => v.index).ToList();
        }
    }
}
