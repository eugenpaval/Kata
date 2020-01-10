using NUnit.Framework;
using Kata;

namespace KataTests
{
    [TestFixture]
    public class SumStringsTests
    {
        [Test]
        [TestCase("9999999999", "1", ExpectedResult = "10000000000")]
        [TestCase("", null, ExpectedResult = null)]
        public string Test(string a, string b)
        {
            return SumStrings.sumStrings(a, b);
        }
    }
}