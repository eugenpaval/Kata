using System;
using System.Linq;
using Kata;
using NUnit.Framework;

namespace KataTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void Repeat()
        {
            var v = new System.Random().Next();
            var s = Stream.Repeat(v);
            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(v, s.Head, "iterate with exponentiation of integers");
                s = s.Tail.Value;
            }
        }

        [Test]
        public void Iterate()
        {
            long multiplier = new System.Random().Next(9);
            Func<long, long> multiply = x => x * multiplier;
            long multiplied = multiplier;
            var expStream = Stream.Iterate(multiply, multiplied);

            Func<string, string> concatenate = x => x + " ";
            var concatenated = "";
            var addWSStream = Stream.Iterate(concatenate, concatenated);

            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(multiplied, expStream.Head, "iterate with exponentiation of integers");
                expStream = expStream.Tail.Value;
                multiplied = multiply(multiplied);

                Assert.AreEqual(concatenated, addWSStream.Head, "iterate with adding whitespaces");
                addWSStream = addWSStream.Tail.Value;
                concatenated = concatenate(concatenated);
            }
        }

        [Test]
        public void Cycle()
        {
            var r = new System.Random();
            var a = Enumerable.Range(0, 20).Select(i => r.Next()).ToArray();
            var s = Stream.Cycle(a);
            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(a[i % a.Length], s.Head, "cycle should repeat the enumerable");
                s = s.Tail.Value;
            }
        }

        [Test]
        public void From()
        {
            var v = new System.Random().Next();
            var s = Stream.From(v);
            for (var i = v; i < v + 100; i++)
            {
                Assert.AreEqual(i, s.Head, "from should count");
                s = s.Tail.Value;
            }
        }

        [Test]
        public void FromThen()
        {
            var r = new System.Random();
            var v = r.Next();
            var d = r.Next(200);
            var s = Stream.FromThen(v, d);
            for (var i = v; i < v + 100 * d; i += d)
            {
                Assert.AreEqual(i, s.Head, "fromThen should count by step");
                s = s.Tail.Value;
            }
        }
    }

    [TestFixture]
    public class ReductionAndModificationTests
    {
        [Test]
        public void Foldr()
        {
            var v = new System.Random().Next();
            var a = Stream.Repeat(v).Foldr<int, Stream<int>>((x, r) => Stream.Cons(x + 1, () => r())).Take(10).ToArray();
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(a[i], v + 1, "folding should work, with lazy tail");
            }
        }

        [Test]
        public void Filter()
        {
            var v = new System.Random().Next();
            var a = Stream.From(v).Filter(x => x % 2 == 0).Take(10).ToArray();
            foreach (var i in a)
            {
                Assert.AreEqual(0, i % 2, "filtering odds with Filter");
            }
        }

        [Test]
        public void Take()
        {
            var s = Stream.From(0);
            for (var i = -2; i <= 2; i++)
            {
                Assert.AreEqual(Math.Max(0, i), s.Take(i).Count(), "Take should get the correct size");
            }
        }

        [Test]
        public void Drop()
        {
            var s = Stream.From(0);
            for (var i = -2; i <= 2; i++)
            {
                Assert.AreEqual(Math.Max(0, i), s.Drop(i).Head, "Drop should drop the correct amount");
            }
        }

        [Test]
        public void ZipWith()
        {
            var s = Stream.From(0).ZipWith((x, y) => x * 2 + y, Stream.Repeat(42));
            var t = Stream.FromThen(42, 2);

            for (var i = 0; i < 20; i++)
            {
                Assert.AreEqual(t.Head, s.Head, "ZipWith should work");
                s = s.Tail.Value;
                t = t.Tail.Value;
            }
        }

        [Test]
        public void FMap()
        {
            var s = Stream.FromThen(42, 2).FMap(x => (x - 42) / 2);
            var t = Stream.From(0);

            for (var i = 0; i < 20; i++)
            {
                Assert.AreEqual(t.Head, s.Head, "FMap should work");
                s = s.Tail.Value;
                t = t.Tail.Value;
            }
        }
    }
}
