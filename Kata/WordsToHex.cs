using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kata
{
    public class WordsToHexClass
    {
        public static string[] WordsToHex(string words)
        {
            return Regex.Matches(words, "\\S+").Select(m => m.Value).Select(ToHex).ToArray();
        }

        private static string ToHex(string w)
        {
            var result = 0;
            var i = 0;
            for (; i < 3; ++i)
            {
                result <<= 8;
                
                var b = i < w.Length ? w[i] : 0;
                result |= b;
            }

            return $"#{Convert.ToString(result,16)}";
        }
    }
}
