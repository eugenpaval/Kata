using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers4v4
    {
        private readonly int[][] _matrix =
        {
            new int[4],
            new int[4],
            new int[4],
            new int[4]
        };

        private readonly (int Clue, int[][] PinnedPositions)[] _clueToPinnedPositions =
        {
            (
                1, 
                new[]
                {
                    new[] { 4, 0, 0, 0}
                }
            ),
            (
                2,
                new[]
                {
                    new[] {1, 4, 0, 0},
                    new[] {2, 4, 0, 0},
                    new[] {2, 1, 4, 3},
                    new[] {3, 4, 0, 0}
                }
            ),
            (
                3,
                new[]
                {
                    new[] {1, 2, 4, 3},
                    new[] {1, 3, 4, 2},
                    new[] {1, 3, 2, 4},
                    new[] {2, 3, 4, 1},
                    new[] {2, 1, 3, 4},
                }
            ),
            (
                1,
                new[]
                {
                    new[] { 4, 0, 0, 0}
                }
            ),
        };

        private Skyscrapers4v4(int[] clues)
        {
            VantagePoints = clues.Select((i, c) => new VantagePoint(i, c));
        }

        IEnumerable<VantagePoint> VantagePoints { get; }

        public static int[][] SolvePuzzle(int[] clues)
        {
            var block = new Skyscrapers4v4(clues);

            return null;
        }

        private bool IsVisibleFrom(int i, int j, VantagePointType vp)
        {
            switch (vp)
            {
                case VantagePointType.Left:
                    return j == 0 || Enumerable.Range(0, j).All(x => _matrix[i][x] < _matrix[i][j]);
                case VantagePointType.Down:
                    return i == 0 || Enumerable.Range(0, i).All(x => _matrix[x][i] < _matrix[i][j]);
                case VantagePointType.Right:
                    return j == 3 || Enumerable.Range(0, 3 - j).All(x => _matrix[i][3-x] < _matrix[i][j]);
                case VantagePointType.Up:
                    return i == 3 || Enumerable.Range(0, 3 - i).All(x => _matrix[3 - x][j] < _matrix[i][j]);
            }

            return false;
        }

        private bool IsUniqueByRowAndColumn(int i, int j)
        {
            return Enumerable.Range(0, 4)
                       .Select(x => _matrix[i][x]).Count(x => x == _matrix[i][j]) == 1
                   && Enumerable.Range(0, 4)
                       .Select(x => _matrix[x][j]).Count(x => x == _matrix[i][j]) == 1;
        }

        private IEnumerable<List<int>> PermutateScrapers(int[] pinnedPositions)
        {
            // A zero in pinnedPositions means allowed to permutate otherwise that position will be pinned to the value contained
            var permutate = new[] {1, 2, 3, 4}.Except(pinnedPositions).Where(v => v != 0).ToArray();
            foreach (var p in PermutateHelper(permutate))
            {
                var r = new List<int>(pinnedPositions);
                for (var (i, j) = (0, 0); i < 4; ++i)
                    if (r[i] == 0)
                        r[i] = p[j++];

                yield return r;
            }
        }

        private IEnumerable<List<int>> PermutateHelper(int[] root)
        {
            if (root.Length == 1)
                yield return new List<int> {root[0]};
            else
                foreach (var s in root)
                {
                    var r = new List<int> {s};
                    foreach (var p in PermutateHelper(root[1..]))
                    {
                        r.AddRange(p);
                        yield return r;
                    }
                }
        }
    }

    class VantagePoint
    {
        public VantagePoint(int index, int value)
        {
            VantagePointType = (VantagePointType)(index / 4);
            FromHereISee = value;
        } 

        public VantagePointType VantagePointType { get; }
        public int FromHereISee { get; }
    }

    enum VantagePointType
    {
        Down,
        Right,
        Up,
        Left
    }
}
