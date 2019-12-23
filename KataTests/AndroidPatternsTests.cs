using NUnit.Framework;
using Kata;

namespace KataTests
{
    [TestFixture]
    public class AndroidPatternsTests
    {
        //[Test, Description("Android Patterns -- Tests")]
        //[TestCase('A', 0, ExpectedResult = 0)]
        //[TestCase('A', 10, ExpectedResult = 0)]
        //[TestCase('B', 1, ExpectedResult = 1)]
        //[TestCase('C', 2, ExpectedResult = 5)]
        //[TestCase('D', 3, ExpectedResult = 37)]
        //[TestCase('E', 4, ExpectedResult = 256)]
        //[TestCase('E', 8, ExpectedResult = 23280)]
        //public int ExampleTests(char firstDot, int length)
        //{
        //    return AndroidPatterns.CountPatternsFrom(firstDot, length);
        //}
    }

    [TestFixture]
    public class AndroidPatternsSimpleTests
    {
        [Test, Description("Android Patterns - Simple Tests")]
        [TestCase('A', 3, ExpectedResult = 6)]
        public int TestCount(char node, int length)
        {
            return new AndroidPatterns().Count(node, length);
        }
    }
}