using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            return new MatrixPuzzle().PermutateMatrix();
        }
    }

    class MatrixPuzzle
    {
        public const int Dimension = 6;

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

            BuildMatrixLines(matrix);
            FinalizeMatrix(ref matrix);

            return matrix;
        }

        private void BuildMatrixLines(int[][] matrix)
        {
            var columns = matrix.SelectMany(lines => lines.Select(x => x))
                .Select((elements, index) => (elements, index))
                .GroupBy(x => x.index % Dimension, x => x.elements)
                .Select(x => new {Position = x.Key, Vector = x.ToArray(), Clues = MatrixExtensions.CluesForColumn(x.Key)});

            var vectors = matrix
                .Select
                (
                    (elements, position) => new
                    {
                        Position = position, Vector = matrix[position], Clues = MatrixExtensions.CluesForLine(position)
                    }
                )
                .Union(columns)
                .Select(x => (x.Position, x.Vector, x.Clues));

            BuildMatrix2(matrix, vectors);
        }

        private bool BuildMatrix2(int[][] matrix, IEnumerable<(int position, int[] permutateVector, (int clue1, int clue2) clues)> clues)
        {
            var lClue = clues.FirstOrDefault();

            if (lClue != default)
            {
                var permutations = lClue.permutateVector.StartPermutateForClues(lClue.clues.clue1, lClue.clues.clue2);
                foreach (var p in permutations)
                {
                    if (matrix.IsCompatible(p, lClue.position))
                    {
                        if (BuildMatrix2(matrix, clues.Skip(1)))
                        {
                            for (var i = 0; i < Dimension; ++i)
                                if (matrix[lClue.position][i] != 0)
                                    matrix[lClue.position][i] = p[i];

                            return true;
                        }
                    }
                }
            }

            return true;
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
            for (var k = 0; k < MatrixPuzzle.Dimension; ++k)
                if (permutation[k] != matrix[line][k] && !matrix.IsRestricted(line, k, k, permutation.ToArray()))
                    matrix[line][k] = permutation[k];

            var retVal = matrix[line].All(x => x != 0);
            return retVal;
        }
    }

    public static class MatrixExtensions
    {
        private static readonly (int, int)[] _lineClues = 
        {
            (23, 6),
            (22, 7),
            (21, 8),
            (20, 9),
            (19, 10),
            (18, 11)
        };

        private static readonly (int, int)[] _columnClues = 
        {
            (0, 17),
            (1, 16),
            (2, 15),
            (3, 14),
            (4, 13),
            (5, 12)
        };

        public static (int, int) CluesForLine(int line)
        {
            return _lineClues[line];
        }

        public static (int, int) CluesForColumn(int column)
        {
            return _columnClues[column];
        }

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

        public static IEnumerable<int[]> StartPermutateForClues(this int[] startingValues, int clue1, int clue2)
        { 
            var startingList = new List<int> { 1, 2, 3, 4, 5, 6 }.Except(startingValues).Where(v => v != 0).ToList();

            var result = startingList.PermutateList()
                .Select
                (
                    p =>
                    {
                        var partial = new List<int>(startingList);
                        for (var (i, k) = (0, 0); i < partial.Count; ++i)
                            if (partial[i] == 0)
                                partial[i] = p[k++];

                        return partial;
                    }
                );

            if (clue1 != 0)
                result = result.Where(l => l.CountVisible() == clue1);

            if (clue2 != 0)
                result = result.Where(l => l.CountVisible(true) == clue2)
                    .Select
                    (
                        l =>
                        {
                            l.Reverse();
                            return l;
                        }
                    );

            return result.Select(l => l.ToArray());
        }

        public static IEnumerable<List<int>> PermutateList(this List<int> current)
        {
            if (current.Count == 0)
                yield return current;

            foreach (var e in current)
            {
                foreach (var p in current.Where(x => x != e).ToList().PermutateList())
                {
                    var result = new List<int> { e };
                    result.AddRange(p);

                    yield return result;
                }
            }
        }

        public static int CountVisible(this List<int> list, bool reversed = false)
        {
            var (count, max) = (0, 0);
            var (start, increment) = reversed ? (list.Count - 1, -1) : (0, 1);
            var iter = 0;

            for (var i = start; iter++ < list.Count; i += increment)
            {
                if (max < list[i])
                {
                    max = list[i];
                    ++count;
                }
            }

            return count;
        }

        public static bool IsCompatible(this int[][] matrix, int[] permutation, int position, bool column = false)
        {
            for (var i = 0; i < matrix[position].Length; ++i)
            {
                if (matrix[position][i] != 0 && matrix[position][i] != permutation[i])
                    return false;

                if (matrix.IsRestricted(position, i, i, permutation))
                    return false;
            }

            return true;
        }

        public static 
    }
}
