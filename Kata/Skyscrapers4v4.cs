using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers4v4
    {
        IEnumerable<VantagePoint> VantagePoints { get; }

        public static int[][] SolvePuzzle(int[] clues)
        {
            var mp = new MatrixPuzzle();

            return mp.PermutateMatrix(clues);
        }
    }

    class MatrixPuzzle
    {
        private readonly int[][] _matrix =
        {
            new int[4],
            new int[4],
            new int[4],
            new int[4]
        };

        public int[][] PermutateMatrix(int[] clues)
        {
            var vpoints = clues.Select((c, i) => new VantagePoint(i, c));
            return PermutateMatrix(_matrix, vpoints);
        }

        private int[][] PermutateMatrix(int[][] matrix, IEnumerable<VantagePoint> vpoints)
        {
            if (!vpoints.Any())
                return matrix;

            foreach (var vp in vpoints)
            {
                if (PermutateVector(matrix, vp))
                    matrix = PermutateMatrix((int[][])matrix.Clone(), vpoints.Skip(1));
            }

            matrix = FinalizeMatrix(matrix);
            return matrix;

            throw new Exception("There is no puzzle solution");
        }

        private int[][] FinalizeMatrix(int[][] matrix, int startLine = 0)
        {
            for (var i = startLine; i < 4; ++i)
            {
                foreach (var p in matrix[i].PermutateScrapers())
                    if (FillMatrixLine(matrix, p, i))
                        break;

                matrix = FinalizeMatrix(matrix, ++startLine);
            }

            return matrix;
        }

        private static bool FillMatrixLine(int[][] matrix, List<int> permutation, int line)
        {
            var clonedLine = (int[]) matrix[line].Clone();

            for (var k = 0; k < 4; ++k)
                if (permutation[k] != matrix[line][k] && !matrix.IsRestricted(line, k, k, matrix[line]))
                    matrix[line][k] = permutation[k];

            var retVal = matrix[line].All(x => x != 0);

            if (!retVal)
                matrix[line] = clonedLine;

            return retVal;
        }

        private bool PermutateVector(int[][] matrix, VantagePoint vp)
        {
            foreach (var pm in vp.PermutationMasks)
            {
                var posInMask = 0;

                switch (vp.VantagePointType)
                {
                    case VantagePointType.Down:
                        for (var i = 0; i < 4; ++i)
                        {
                            if (!matrix.IsRestricted(i, vp.Column, posInMask, pm))
                                matrix[i][vp.Column] = pm[i];
                            else
                                break;
                        }
                        break;

                    case VantagePointType.Right:
                        for (var j = 3; j >= 0; --j)
                        {
                            if (!matrix.IsRestricted(vp.Line, j, posInMask, pm))
                                matrix[vp.Line][j] = pm[3 - j];
                            else
                                break;
                        }
                        break;

                    case VantagePointType.Up:
                        for (var i = 3; i >= 0; --i)
                        {
                            if (!matrix.IsRestricted(i, vp.Column, posInMask, pm))
                                matrix[i][vp.Column] = pm[3 - i];
                            else
                                break;
                        }
                        break;

                    case VantagePointType.Left:
                        for (var j = 0; j < 4; ++j)
                        {
                            if (!matrix.IsRestricted(vp.Line, j, posInMask, pm))
                                matrix[vp.Line][j] = pm[j];
                            else
                                break;
                        }
                        break;
                }

                ++posInMask;
            }

            return true;
        }
    }

    class VantagePoint
    {
        private static readonly (int Clue, int[][] PinnedPositions)[] _pMasks =
        {
            (
                0,
                new[]
                {
                    new[] { 0, 0, 0, 0}
                }
            ),
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
                    new[] {0, 4, 0, 0},
                    new[] {2, 1, 4, 3},
                    new[] {3, 0, 0, 0},
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
                    new[] {2, 3, 1, 4}
                }
            ),
            (
                4,
                new[]
                {
                    new[] { 1, 2 ,3, 4}
                }
            ),
        };

        private static readonly (int Line, int Column)[] _pos =
        {
            (0, 0),
            (0, 1),
            (0, 2),
            (0, 3),
            (0, 3),
            (1, 3),
            (2, 3),
            (3, 3),
            (3, 3),
            (3, 2),
            (3, 1),
            (3, 0),
            (3, 0),
            (2, 0),
            (1, 0),
            (0, 0)
        };

        public VantagePoint(int index, int value)
        {
            Line = _pos[index].Line;
            Column = _pos[index].Column;
            VantagePointType = (VantagePointType)(index / 4);
            Clue = value;
        }

        public int Column { get; }
        public int Line { get; }
        public VantagePointType VantagePointType { get; }
        public int Clue { get; }

        public IEnumerable<int[]> PermutationMasks => _pMasks[Clue].PinnedPositions;
    }

    enum VantagePointType
    {
        Down,
        Right,
        Up,
        Left
    }

    static class MatrixExtensions
    {
        public static bool IsRestricted(this int[][] matrix, int i, int j, int posInMask, int[] mask)
        {
            var mVal = matrix[i][j];
            var val = mask[posInMask];

            if (mVal == val || mask.IsAllowedByMask(posInMask, val))
                return false;

            if (matrix[i].All(x => x != val) && matrix.Select(lines => lines[j]).All(x => x != val))
                return false;

            return true;
        }

        public static bool IsAllowedByMask(this int[] mask, int pos, int val)
        {
            if (mask[pos] == val)
                return true;

            if (mask[pos] == 0)
                return mask.Count(x => x == val) == 1;

            return false;
        }

        public static IEnumerable<List<int>> PermutateScrapers(this int[] pinnedPositions)
        {
            // A zero in pinnedPositions means allowed to permutate otherwise that position will be pinned to the value contained
            var permutate = new[] { 1, 2, 3, 4 }.Except(pinnedPositions).Where(v => v != 0).ToArray();
            foreach (var p in PermutateHelper(permutate))
            {
                var r = new List<int>(pinnedPositions);
                for (var (i, j) = (0, 0); i < 4; ++i)
                    if (r[i] == 0)
                        r[i] = p[j++];

                yield return r;
            }
        }

        private static IEnumerable<List<int>> PermutateHelper(this int[] root)
        {
            if (root.Length == 1)
                yield return new List<int> { root[0] };
            else
                foreach (var s in root)
                {
                    var r = new List<int> { s };
                    foreach (var p in PermutateHelper(root[1..]))
                    {
                        r.AddRange(p);
                        yield return r;
                    }
                }
        }
    }
}
