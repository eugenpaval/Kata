using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers4v4
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            return new MatrixPuzzle(clues).PermutateMatrix();
        }
    }

    class MatrixPuzzle
    {
        public MatrixPuzzle(int[] clues)
        {
            VantagePoints = clues.Select((c, i) => new VantagePoint(i, c)).Where(vp => vp.Clue != 0);
        }

        IEnumerable<VantagePoint> VantagePoints { get; }

        public int[][] PermutateMatrix()
        {
            int[][] matrix =
            {
                new int[4],
                new int[4],
                new int[4],
                new int[4]
            };

            BuildMatrix(ref matrix, VantagePoints);
            FinalizeMatrix(matrix);

            return matrix;
        }

        private bool BuildMatrix(ref int[][] matrix, IEnumerable<VantagePoint> vPoints)
        {
            var vp = vPoints.FirstOrDefault();

            if (vp != null)
            {
                foreach (var pm in vp.PermutationMasks)
                {
                    var originalMatrix = matrix.MakeClone();

                    if (BuildMatrixFrom(ref matrix, pm, vp))
                        return true;

                    matrix = originalMatrix;
                }

                return false;
            }

            return true;
        }

        private bool BuildMatrixFrom(ref int[][] matrix, int[] pm, VantagePoint vp)
        {
            if (PermutateVector(matrix, vp.Line, vp.Column, vp.VantagePointType, pm))
                return BuildMatrix(ref matrix, VantagePoints.SkipWhile(p => p.Line != vp.Line || p.Column != vp.Column).Skip(1));

            return false;
        }

        private bool PermutateVector(int[][] matrix, int line, int column, VantagePointType vpType, int[] pm)
        {
            var success = true;

            switch (vpType)
            {
                case VantagePointType.Down:
                    for (var i = 0; i < 4; ++i)
                    {
                        var mPin = pm[i];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(i, column, i, pm))
                            matrix[i][column] = mPin;
                        else
                        {
                            success = false;
                            for (var k = 0; k < i; ++k)
                                if (pm[k] == matrix[k][column])
                                    matrix[k][column] = 0;
                            break;
                        }
                    }
                    break;

                case VantagePointType.Right:
                    for (var j = 3; j >= 0; --j)
                    {
                        var mPin = pm[3-j];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(line, j, 3 - j, pm))
                            matrix[line][j] = mPin;
                        else
                        {
                            success = false;
                            for (var k = 3; k > j; --k)
                                if (pm[3 - k] == matrix[line][k])
                                    matrix[line][k] = 0;
                            break;
                        }
                    }
                    break;

                case VantagePointType.Up:
                    for (var i = 3; i >= 0; --i)
                    {
                        var mPin = pm[3-i];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(i, column, 3 - i, pm))
                            matrix[i][column] = mPin;
                        else
                        {
                            success = false;
                            for (var k = 3; k > i; --k)
                                if (pm[3-k] == matrix[k][column])
                                    matrix[k][column] = 0;
                            break;
                        }
                    }
                    break;

                case VantagePointType.Left:
                    for (var j = 0; j < 4; ++j)
                    {
                        var mPin = pm[j];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(line, j, j, pm))
                            matrix[line][j] = mPin;
                        else
                        {
                            success = false;
                            for (var k = 0; k < j; ++k)
                                if (pm[k] == matrix[line][k])
                                    matrix[line][k] = 0;
                            break;
                        }
                    }
                    break;
            }

            return success;
        }

        private int[][] FinalizeMatrix(int[][] matrix, int startLine = 0)
        {
            for (var i = startLine; i < 4; ++i)
            {
                foreach (var p in matrix[i].PermutateLine())
                    if (FillMatrixLine(matrix, p, i))
                    {
                        matrix = FinalizeMatrix(matrix, ++startLine);
                        break;
                    }

                matrix = FinalizeMatrix(matrix, --startLine);
            }

            return matrix;
        }

        private static bool FillMatrixLine(int[][] matrix, List<int> permutation, int line)
        {
            var clonedLine = matrix[line].MakeClone();

            for (var k = 0; k < 4; ++k)
                if (permutation[k] != matrix[line][k] && !matrix.IsRestricted(line, k, k, permutation.ToArray()))
                    matrix[line][k] = permutation[k];

            var retVal = matrix[line].All(x => x != 0);

            if (!retVal)
                matrix[line] = clonedLine;

            return retVal;
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

    public static class MatrixExtensions
    {
        public static int[][] MakeClone(this int[][] matrix)
        {
            var copy = new int[matrix.Length][];

            for (var i = 0; i < matrix.Length; ++i)
            {
                copy[i] = new int[matrix[i].Length];
                for (var j = 0; j < copy[i].Length; ++j)
                    copy[i][j] = matrix[i][j];
            }

            return copy;
        }

        public static int[] MakeClone(this int[] array)
        {
            var copy = new int[array.Length];
            for (var i = 0; i < 4; ++i)
                copy[i] = array[i];

            return copy;
        }

        public static bool IsRestricted(this int[][] matrix, int i, int j, int posInMask, int[] mask)
        {
            var mVal = matrix[i][j];
            var val = mask[posInMask];

            if (mVal != 0 && mVal != val)
                return true;

            if (mVal == val || mask.IsAllowedByMask(posInMask, mVal))
                return false;

            if (matrix[i].All(x => x != val) && matrix.Select(lines => lines[j]).All(x => x != val))
                return false;

            return true;
        }

        public static bool IsAllowedByMask(this int[] mask, int pos, int val)
        {
            if (mask[pos] == 0)
                return mask.Count(x => x == val) == 0;

            if (mask[pos] == val)
                return true;

            return false;
        }

        public static IEnumerable<List<int>> PermutateLine(this int[] pinnedPositions)
        {
            // A zero in pinnedPositions means allowed to permutate otherwise that position will be pinned to the value contained
            var permutate = new[] {1, 2, 3, 4}.Except(pinnedPositions).Where(v => v != 0).ToList();

            foreach (var p in PermutateNonPinned(permutate))
            {
                var result = new List<int>();
                var k = 0;
                for (var i = 0; i < 4; ++i)
                    result.Add(pinnedPositions[i] != 0 ? pinnedPositions[i] : p[k++]);

                yield return result;
            }
        }

        public static IEnumerable<List<int>> PermutateNonPinned(List<int> permutate)
        {
            if (permutate.Count == 1)
                yield return permutate;
            else
            {
                foreach (var f in permutate)
                {
                    var list = PermutateNonPinned(permutate.Where(e => e != f).ToList());

                    foreach (var l in list)
                    {
                        var root = new List<int> {f};
                        root.AddRange(l);
                        yield return root;
                    }
                }
            }
        }
    }
}
