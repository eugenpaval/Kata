using System;
using System.Diagnostics;
using NUnit.Framework;
using Kata;

namespace KataTests
{
    [TestFixture]
    public class Skyscrapers4v4Tests
    {
        [Test]
        public void SolvePuzzle1()
        {
            var clues = new[]
            {
                3, 2, 2, 3, 2, 1,
                1, 2, 3, 3, 2, 2,
                5, 1, 2, 2, 4, 3,
                3, 2, 1, 2, 2, 4
            };

            var expected = new[]
            {
                new[] {2, 1, 4, 3, 5, 6},
                new[] {1, 6, 3, 2, 4, 5},
                new[] {4, 3, 6, 5, 1, 2},
                new[] {6, 5, 2, 1, 3, 4},
                new[] {5, 4, 1, 6, 2, 3},
                new[] {3, 2, 5, 4, 6, 1}
            };

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SolvePuzzle2()
        {
            var clues = new[]
            {
                0, 0, 0, 2, 2, 0,
                0, 0, 0, 6, 3, 0,
                0, 4, 0, 0, 0, 0,
                4, 4, 0, 3, 0, 0
            };

            var expected = new[]
            {
                new[] {5, 6, 1, 4, 3, 2},
                new[] {4, 1, 3, 2, 6, 5},
                new[] {2, 3, 6, 1, 5, 4},
                new[] {6, 5, 4, 3, 2, 1},
                new[] {1, 2, 5, 6, 4, 3},
                new[] {3, 4, 2, 5, 1, 6}
            };

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Solve44()
        {
            var clues = new []
            {
                0, 0, 1, 2,
                0, 2, 0, 0,
                0, 3, 0, 0,
                0, 1, 0, 0
            };

            var expected = new[]
            {
                new[] {2, 1, 4, 3},
                new[] {3, 4, 1, 2},
                new[] {4, 2, 3, 1},
                new[] {1, 3, 2, 4}
            };

            var actual = Skyscrapers.SolvePuzzle(clues);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}