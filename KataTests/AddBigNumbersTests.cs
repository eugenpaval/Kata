using Kata;
using NUnit.Framework;

namespace KataTests
{
    [TestFixture]
    public class AddBigNumbersTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual("103397744329", AddBigNumbers.Add("7901564791", "95496179538"));
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual("93397744329", AddBigNumbers.Add("7901564791", "85496179538"));
        }
    }
}