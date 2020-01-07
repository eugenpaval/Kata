using Kata;
using NUnit.Framework;

namespace KataTests
{
    [TestFixture]
    public class LongestPalindromeTests
    {
        [TestCase("madam", ExpectedResult = "madam")]
        [TestCase("dde", ExpectedResult = "dd")]
        [TestCase("banana", ExpectedResult = "anana")]
        [TestCase("ababbab", ExpectedResult = "babbab")]
        [TestCase("xx", ExpectedResult = "xx")]
        [TestCase("", ExpectedResult = "")]
        [TestCase("x", ExpectedResult = "x")]
        [TestCase("abcdefgh", ExpectedResult = "a")]
        public string Test(string text)
        {
            return LongestPalindromeContainer.LongestPalindrome(text);
        }
    }
}