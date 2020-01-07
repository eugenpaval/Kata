using System.Linq;

namespace Kata
{
    public class LongestPalindromeContainer
    {
        public static string LongestPalindrome(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length == 1)
                return text;

            var (_max, _l) = ("", text.Length);

            string Reduce(string s, int l, int r)
            {
                if (l - 1 >= 0 && r + 1 < _l && s[l - 1] == s[r + 1])
                    return Reduce(s, l - 1, r + 1);

                return s.Substring(l, r - l + 1);
            }

            var (s1, s2, sMax) = ("", "", "");

            foreach (var i in Enumerable.Range(0, _l - 1))
            {
                (s1, s2) = (Reduce(text, i, i), i + 1 < _l && text[i] == text[i + 1] ? Reduce(text, i, i + 1) : "");
                sMax = s1.Length >= s2.Length ? s1 : s2;
                _max = _max.Length >= sMax.Length ? _max : sMax;
            }


            return _max;
        }
    }
}
