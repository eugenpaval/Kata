using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            return new MatrixPuzzle(clues).PermutateMatrix();
        }
    }

    class MatrixPuzzle
    {
        public const int Dimension = 6;
        public MatrixPuzzle(int[] clues)
        {
            VantagePoints = clues.Select((c, i) => new VantagePoint(i, c)).Where(vp => vp.Clue != 0);
        }

        IEnumerable<VantagePoint> VantagePoints { get; }

        public int[][] PermutateMatrix()
        {
            int[][] matrix =
            {
                new int[MatrixPuzzle.Dimension],
                new int[MatrixPuzzle.Dimension],
                new int[MatrixPuzzle.Dimension],
                new int[MatrixPuzzle.Dimension],
                new int[MatrixPuzzle.Dimension],
                new int[MatrixPuzzle.Dimension],
            };

            BuildMatrix(ref matrix, VantagePoints);
            FinalizeMatrix(ref matrix);

            return matrix;
        }

        private bool BuildMatrix(ref int[][] matrix, IEnumerable<VantagePoint> vPoints)
        {
            var vp = vPoints.FirstOrDefault();

            if (vp != null)
            {
                foreach (var pm in vp.PermutationMasks)
                {
                    var matrixCopy = matrix.MakeClone();

                    if (BuildMatrixFrom(ref matrixCopy, pm, vp))
                    {
                        matrix = matrixCopy;
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        private bool BuildMatrixFrom(ref int[][] matrix, int[] pm, VantagePoint vp)
        {
            if (PermutateVector(matrix, vp.Line, vp.Column, vp.VantagePointType, pm))
                return BuildMatrix(ref matrix, VantagePoints.SkipWhile(p => p.Line != vp.Line || p.Column != vp.Column || p.VantagePointType != vp.VantagePointType).Skip(1));

            return false;
        }

        private bool PermutateVector(int[][] matrix, int line, int column, VantagePointType vpType, int[] pm)
        {
            var success = true;

            switch (vpType)
            {
                case VantagePointType.Down:
                    for (var i = 0; i < MatrixPuzzle.Dimension; ++i)
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
                    for (var j = MatrixPuzzle.Dimension-1; j >= 0; --j)
                    {
                        var mPin = pm[MatrixPuzzle.Dimension - 1 - j];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(line, j, MatrixPuzzle.Dimension - j - 1, pm))
                            matrix[line][j] = mPin;
                        else
                        {
                            success = false;
                            for (var k = MatrixPuzzle.Dimension-1; k > j; --k)
                                if (pm[MatrixPuzzle.Dimension -1 - k] == matrix[line][k])
                                    matrix[line][k] = 0;
                            break;
                        }
                    }
                    break;

                case VantagePointType.Up:
                    for (var i = MatrixPuzzle.Dimension-1; i >= 0; --i)
                    {
                        var mPin = pm[MatrixPuzzle.Dimension - 1 - i];
                        if (mPin == 0)
                            continue;

                        if (!matrix.IsRestricted(i, column, MatrixPuzzle.Dimension - 1 - i, pm))
                            matrix[i][column] = mPin;
                        else
                        {
                            success = false;
                            for (var k = MatrixPuzzle.Dimension-1; k > i; --k)
                                if (pm[MatrixPuzzle.Dimension - 1 - k] == matrix[k][column])
                                    matrix[k][column] = 0;
                            break;
                        }
                    }
                    break;

                case VantagePointType.Left:
                    for (var j = 0; j < MatrixPuzzle.Dimension; ++j)
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

        private void FinalizeMatrix(ref int[][] matrix)
        {
            var linesToProcess = matrix.Select((elements, position) => (elements, position)).Where(x => x.elements.Any(e => e == 0)).ToList();
            FinalizeMatrix(ref matrix, linesToProcess);
        }

        private bool FinalizeMatrix(ref int[][] matrix, IEnumerable<(int[] elements, int position)> linesToProcess)
        {
            var line = linesToProcess.FirstOrDefault();

            if (line != default)
            {
                var permutations = line.elements.MakeClone();
                foreach (var p in permutations.PermutateLine())
                {
                    var matrixCopy = matrix.MakeClone();

                    if (FillMatrixLine(matrixCopy, p, line.position))
                    {
                        if (FinalizeMatrix(ref matrixCopy, linesToProcess.Skip(1)))
                        {
                            matrix = matrixCopy;
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        private static bool FillMatrixLine(int[][] matrix, List<int> permutation, int line)
        {
            //var clonedLine = matrix[line].MakeClone();

            for (var k = 0; k < MatrixPuzzle.Dimension; ++k)
                if (permutation[k] != matrix[line][k] && !matrix.IsRestricted(line, k, k, permutation.ToArray()))
                    matrix[line][k] = permutation[k];

            var retVal = matrix[line].All(x => x != 0);

            //if (!retVal)
            //    matrix[line] = clonedLine;

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
                    new[] {0,0,0,0,0,0}
                }
            ),
            (
                1,
                new[]
                {
                    new[] {6,0,0,0,0,0}
                }
            ),
            (
                2,
                new[]
                {
                    new[] {0,6,0,0,0,0},
                    new[] {5,0,0,0,0,0},
                }
            ),
            (
                3,
                new[]
                {
                    new[] {0,5,6,0,0,0},
                    new[] {0,5,0,6,0,0},
                    new[] {0,5,0,0,6,0},
                    new[] {0,5,0,0,0,6},
                    new[] {2,1,5,6,0,0},
                    new[] {2,3,6,0,0,0},
                    new[] {2,4,6,0,0,0},
                    new[] {3,0,0,5,6,4},
                    new[] {3,0,5,4,6,0},
                    new[] {3,0,5,6,4,0},
                    new[] {3,0,5,6,0,4},
                }
            ),
            (
                4,
                new[]
                {
                    new[] {1,0,5,0,0,6},
                    new[] {1,0,5,0,6,0},
                    new[] {1,0,5,6,0,0},
                    new[] {2,1,0,4,5,6},
                    new[] {2,1,3,5,0,0},
                    new[] {2,1,4,3,5,6},
                    new[] {2,1,4,5,3,6},
                    new[] {2,1,4,5,6,3},
                    new[] {2,1,5,0,0,6},
                    new[] {2,3,5,0,0,6},
                    new[] {2,3,5,0,6,0},
                    new[] {2,3,5,6,0,0},
                    new[] {2,4,5,0,0,6},
                    new[] {2,4,5,0,6,0},
                    new[] {2,4,5,6,0,0},
                    new[] {3,0,0,4,5,6},
                    new[] {3,0,4,0,5,6},
                    new[] {3,0,4,5,0,6},
                    new[] {3,0,4,5,6,0},
                    new[] {3,4,0,5,6,0},
                    new[] {3,4,0,5,0,6},
                    new[] {3,4,5,6,0,0},
                    new[] {3,4,5,0,0,6},
                    new[] {3,4,5,0,6,0},
                    new[] {3,4,5,6,0,0},
                }
            ),
            (
                5,
                new[]
                {
                    new[] {1,2,3,5,0,6},
                    new[] {1,2,3,5,6,0},
                    new[] {2,1,3,4,5,6},
                    new[] {2,3,1,4,5,6},
                    new[] {2,3,4,1,5,6},
                    new[] {2,3,4,5,1,6},
                    new[] {2,3,4,5,6,1},
                }
            ),
            (
                6,
                new[]
                {
                    new[] {1,2,3,4,5,6}
                }
            ),
        };

        private static readonly (int Line, int Column)[] _pos =
        {
            (0, 0),
            (0, 1),
            (0, 2),
            (0, 3),
            (0, 4),
            (0, 5),
            (0, 5),
            (1, 5),
            (2, 5),
            (3, 5),
            (4, 5),
            (5, 5),
            (5, 5),
            (5, 4),
            (5, 3),
            (5, 2),
            (5, 1),
            (5, 0),
            (5, 0),
            (4, 0),
            (3, 0),
            (2, 0),
            (1, 0),
            (0, 0)
        };

        public VantagePoint(int index, int value)
        {
            Line = _pos[index].Line;
            Column = _pos[index].Column;
            VantagePointType = (VantagePointType)(index / MatrixPuzzle.Dimension);
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
            for (var i = 0; i < MatrixPuzzle.Dimension; ++i)
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
            var permutate = new[] {1, 2, 3, 4, 5, 6}.Except(pinnedPositions).Where(v => v != 0).ToList();

            foreach (var p in PermutateNonPinned(permutate))
            {
                var result = new List<int>();
                var k = 0;
                for (var i = 0; i < MatrixPuzzle.Dimension; ++i)
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
