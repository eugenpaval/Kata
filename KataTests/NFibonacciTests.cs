using System.Numerics;
using NUnit.Framework;
using Kata;

namespace KataTests
{
    [TestFixture]
    public class NFibonacciTests
    {
        [Test]
        public void testFib()
        {
            var v = NFibonnacci.fib(-6);
            Assert.AreEqual(new BigInteger(-8), v);
        }

        [Test]
        public void testBigNegative()
        {
            var v = NFibonnacci.fib(-96);
            Assert.AreEqual(new BigInteger(-51680708854858323072m), v);
        }

        [Test]
        public void testBigPositive()
        {
            var v = NFibonnacci.fib(96);
            Assert.AreEqual(new BigInteger(51680708854858323072m), v);
        }

        [Test]
        public void testBigN1()
        {
            var v = NFibonnacci.fib(-15);
            Assert.AreEqual(new BigInteger(610m), v);
        }
    }
}