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
        [TestCase("1.33", ExpectedResult = 1.33)]
        [TestCase("(1.33)", ExpectedResult = 1.33)]
        [TestCase("(-1.33)", ExpectedResult = -1.33)]
        [TestCase("(-1.33)+---1.33", ExpectedResult = -2.66)]
        [TestCase("2*(-3)+   6 / 1", ExpectedResult = 0)]
        [TestCase("1 +- (2 * ((2+3)))", ExpectedResult = -9)]
        [TestCase("+ 25 * 4 - 4 * 25 + .3", ExpectedResult = 0.3)]
        [TestCase("12*-1", ExpectedResult = -12)]
        [TestCase("2*-(1+1)", ExpectedResult = -4)]
        public double TestEvaluation(string expression)
        {
            return Evaluator.Evaluate(expression);
        }
    }
}