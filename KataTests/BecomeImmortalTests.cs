using Kata;
using NUnit.Framework;

[TestFixture]
public class ImmortalTest
{

    [Test]
    public void example()
    {
        //Assert.AreEqual((long)5, Immortal.ElderAge(8, 5, 1, 100));
        //Assert.AreEqual((long)224, Immortal.ElderAge(8, 8, 0, 100007));
        //Assert.AreEqual((long)11925, Immortal.ElderAge(25, 31, 0, 100007));
        //Assert.AreEqual((long)4323, Immortal.ElderAge(5, 45, 3, 1000007));
        //Assert.AreEqual((long)1586, Immortal.ElderAge(31, 39, 7, 2345));
        //Assert.AreEqual((long)808451, Immortal.ElderAge(545, 435, 342, 1000007));
        // You need to run this test very quickly before attempting the actual tests :)
        //Assert.AreEqual((long)5456283, Immortal.ElderAge(28827050410L, 35165045587L, 7109602, 13719506));

        Assert.AreEqual((long) 79, Immortal.ElderAge(39, 557, 16, 328));
    }
}