using System.Numerics;
using Kata;
using NUnit.Framework;

namespace KataTests
{
    [TestFixture]
    public class BooleanOrderTests
    {
        [Test]
        public void SampleTests()
        {
            Assert.AreEqual(new BigInteger(2), new BooleanOrder("tft", "^&").Solve());
            Assert.AreEqual(new BigInteger(16), new BooleanOrder("ttftff", "|&^&&").Solve());
            Assert.AreEqual(new BigInteger(339), new BooleanOrder("ttftfftf", "|&^&&||").Solve());
            Assert.AreEqual(new BigInteger(851), new BooleanOrder("ttftfftft", "|&^&&||^").Solve());
            Assert.AreEqual(new BigInteger(2434), new BooleanOrder("ttftfftftf", "|&^&&||^&").Solve());
            Assert.AreEqual(new BigInteger(945766470799), new BooleanOrder("ttftfftftffttfftftftfftft", "|&^&&||^&&^^|&&||^&&||&^").Solve());
        }
    }
}