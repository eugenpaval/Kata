using System.Reflection.Metadata.Ecma335;
using Kata;
using NUnit.Framework;

namespace KataTests
{
    [TestFixture]
    public class EvaluatorTests
    {
        Evaluator Evaluator { get; } = new Evaluator();

        [Test]
        [TestCase("-1", ExpectedResult = -1)]
        [TestCase("(-1)", ExpectedResult = -1)]
        [TestCase("-(-1)", ExpectedResult = 1)]
        [TestCase("--(-1)", ExpectedResult = -1)]
        [TestCase("1-1", ExpectedResult = 0)]
        [TestCase("-1-(-1)", ExpectedResult = 0)]
        [TestCase("-3*-2", ExpectedResult = 6)]
        [TestCase("-3*--2", ExpectedResult = -6)]
        [TestCase("-3*-(-2+-1)", ExpectedResult = -9)]
        [TestCase("-3*-(-2+-1) +  (-1*-9)", ExpectedResult = 0)]
        [TestCase("(0)+ (-1*-9)", ExpectedResult = 9)]
        [TestCase("-3*-(-2+-1) +  9", ExpectedResult = 0)]
        [TestCase("-3*(3)+9", ExpectedResult = 0)]
        public double TestEvaluation(string expression)
        {
            return Evaluator.Evaluate(expression);
        }
    }
}