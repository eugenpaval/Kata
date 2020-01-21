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

    public class MatrixPuzzle
    {
        public const int Dimension = 4;

        public MatrixPuzzle(int[] clues)
        {
            _clues = clues;
        }

        private readonly int[][] _matrix =
        {
            new int[MatrixPuzzle.Dimension],
            new int[MatrixPuzzle.Dimension],
            new int[MatrixPuzzle.Dimension],
            new int[MatrixPuzzle.Dimension],
            //new int[MatrixPuzzle.Dimension],
            //new int[MatrixPuzzle.Dimension],
        };

        //private static readonly (int, int)[] _lineClues =
        //{
        //    (23, 6),
        //    (22, 7),
        //    (21, 8),
        //    (20, 9),
        //    (19, 10),
        //    (18, 11)
        //};

        //private static readonly (int, int)[] _columnClues =
        //{
        //    (0, 17),
        //    (1, 16),
        //    (2, 15),
        //    (3, 14),
        //    (4, 13),
        //    (5, 12)
        //};
        private static readonly (int, int)[] _lineClues =
        {
            (15, 4),
            (14, 5),
            (13, 6),
            (12, 7),
        };

        private static readonly (int, int)[] _columnClues =
        {
            (0, 11),
            (1, 10),
            (2, 9),
            (3, 8),
        };

        private readonly int[] _clues;

        public (int, int) CluesForLine(int line)
        {
            var (x, y) = _lineClues[line];
            return (_clues[x], _clues[y]);
        }

        public (int, int) CluesForColumn(int column)
        {
            var (x, y) = _columnClues[column];
            return (_clues[x], _clues[y]);
        }

        public MatrixIterator GetIterator(int startPosition, bool isLine)
        {
            return isLine
                ? new LineMatrixIterator(this, startPosition) as MatrixIterator
                : new ColumnMatrixIterator(this, startPosition);
        }

        public int[][] PermutateMatrix()
        {
            BuildMatrixLines();
            FinalizeMatrix();

            return _matrix;
        }
        public int[][] Matrix => _matrix;

        private void BuildMatrixLines()
        {
            var columns = _matrix.SelectMany(lines => lines.Select(x => x))
                .Select((elements, index) => (elements, index))
                .GroupBy(x => x.index % Dimension, x => x.elements)
                .Select(x => new {Position = x.Key, Vector = GetIterator(x.Key, false), Clues = CluesForColumn(x.Key)});

            var vectors = _matrix
                .Select
                (
                    (elements, position) => new
                    {
                        Position = position, Vector = GetIterator(position, true),
                        Clues = CluesForLine(position)
                    }
                )
                .Union(columns)
                .Select(x => (x.Position, x.Vector, x.Clues))
                .Where(x => x.Clues.Item1 != 0 || x.Clues.Item2 != 0);

            BuildMatrix2(vectors);
        }

        private bool BuildMatrix2(IEnumerable<(int position, MatrixIterator matrixVector, (int clue1, int clue2) clues)> clues)
        {
            var clue = clues.FirstOrDefault();

            if (clue != default)
            {
                var permutations = clue.matrixVector.ToArray().StartPermutateForClues(clue.clues.clue1, clue.clues.clue2);
                foreach (var p in permutations)
                {
                    if (clue.matrixVector.CanSetAllValues(p))
                    {
                        clue.matrixVector.SetValues(p);
                        if (BuildMatrix2(clues.Skip(1)))
                            return true;
                        clue.matrixVector.SetValues(new[] { 0, 0, 0, 0 });
                    }
                }

                return false;
            }

            return true;
        }

        private void FinalizeMatrix()
        {
            var linesToProcess = _matrix.Select((elements, position) => (elements, position)).Where(x => x.elements.Any(e => e == 0)).ToList();
            FinalizeMatrix(linesToProcess);
        }

        private bool FinalizeMatrix(IEnumerable<(int[] elements, int position)> linesToProcess)
        {
            var line = linesToProcess.FirstOrDefault();

            if (line != default)
            {
                var matrixLineIter = GetIterator(line.position, true);
                foreach (var p in matrixLineIter.ToArray().StartPermutateForClues(0, 0))
                {
                    if (matrixLineIter.CanSetAllValues(p))
                    {
                        var lineClone = line.elements.MakeClone();
                        matrixLineIter.SetValues(p);
                        
                        if (FinalizeMatrix(linesToProcess.Skip(1)))
                            return true;
                        
                        for (var i = 0; i < lineClone.Length; ++i)
                            _matrix[line.position][i] = lineClone[i];
                    }
                }

                return false;
            }

            return true;
        }
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
            var startingList = new List<int> { 1, 2, 3, 4}.Except(startingValues).Where(v => v != 0).ToList();

            var result = startingList.PermutateList()
                .Select
                (
                    l =>
                    {
                        var r = new List<int>(startingValues.Length);
                        for (var (i, k) = (0, 0); i < r.Capacity; ++i)
                            if (i < startingValues.Length && startingValues[i] != 0)
                                r.Add(startingValues[i]);
                            else
                                r.Add(l[k++]);

                        return r;
                    }
                );

            if (clue1 != 0)
                result = result.Where(l => l.CountVisible() == clue1);

            if (clue2 != 0)
                result = result.Where(l => l.CountVisible(true) == clue2);

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
    }

    public abstract class MatrixIterator
    {
        protected readonly int _startPosition;
        protected readonly MatrixPuzzle _mp;
        protected readonly int[][] _matrix;

        protected MatrixIterator(MatrixPuzzle mp, int startPosition)
        {
            _mp = mp;
            _matrix = mp.Matrix;
            _startPosition = startPosition;
            Count = _matrix.Length;
        }

        public int Count { get; }
        public abstract int GetValue(int position);
        public abstract void SetValue(int position, int value);
        
        public int[] ToArray()
        {
            var result = new int[_matrix.Length];
            for (var i = 0; i < _matrix.Length; ++i)
                result[i] = GetValue(i);

            return result;
        }

        public abstract bool CanSetValue(int position, int value);
        public bool CanSetAllValues(int[] permutation)
        {
            for (var i = 0; i < Count; ++i)
                if (!CanSetValue(i, permutation[i]))
                    return false;

            return true;
        }

        public void SetValues(int[] values)
        {
            for (var i = 0; i < Count; ++i)
                SetValue(i, values[i]);
        }

        public abstract (int i, int j) Coordinates(int i);
    }

    public class LineMatrixIterator : MatrixIterator
    {
        public LineMatrixIterator(MatrixPuzzle mp, int startPosition) : base(mp, startPosition)
        {
        }

        public override int GetValue(int position)
        {
            return _matrix[_startPosition][position];
        }

        public override void SetValue(int position, int value)
        {
            _matrix[_startPosition][position] = value;
        }

        public override bool CanSetValue(int position, int value)
        {
            var (x, y) = Coordinates(position);

            if (_matrix[x][y] != 0 && _matrix[x][y] != value)
                return false;

            var (clueLeft, clueRight) = _mp.CluesForColumn(position);
            var (visLeft, visRight) = (0, 0);
            var (maxLeft, maxRight) = (0, 0);

            for (var i = 0; i < _matrix[_startPosition].Length; ++i)
            {
                var lVal = i != _startPosition ? _matrix[i][y] : value;
                var rVal = _matrix[_matrix[y].Length - i - 1][position];

                if (_matrix[i][y] == value)
                    return false;

                if (lVal == 0 || rVal == 0)
                {
                    visLeft = -1;
                    visRight = -1;
                }

                if (maxLeft < lVal)
                {
                    maxLeft = lVal;
                    if (visLeft != -1)
                        visLeft++;
                }

                if (maxRight < rVal)
                {
                    maxRight = rVal;
                    if (visRight != -1)
                        visRight++;
                }
            }

            return (clueLeft == 0 || visLeft == -1 || visLeft == clueLeft) && (clueRight == 0 || visRight == -1 || visRight == clueRight);
        }

        public override (int i, int j) Coordinates(int i)
        {
            return (_startPosition, i);
        }
    }
    
    public class ColumnMatrixIterator : MatrixIterator
    {
        public ColumnMatrixIterator(MatrixPuzzle mp, int startPosition) : base(mp, startPosition)
        {
        }

        public override int GetValue(int position)
        {
            return _matrix[position][_startPosition];
        }

        public override void SetValue(int position, int value)
        {
            _matrix[position][_startPosition] = value;
        }

        public override bool CanSetValue(int position, int value)
        {
            var (x, y) = Coordinates(position);

            if (_matrix[x][y] != 0 && _matrix[x][y] != value)
                return false;

            var (clueLeft, clueRight) = _mp.CluesForLine(position);
            var (visLeft, visRight) = (0, 0);
            var (maxLeft, maxRight) = (0, 0);

            for (var i = 0; i < _matrix[position].Length; ++i)
            {
                var lVal = i != y ? _matrix[x][i] : value;
                var rVal = _matrix[x][_matrix[x].Length - i - 1];

                if (i == y)
                {
                    lVal = value;
                }

                if (_matrix[x][i] == value)
                    return false;

                if (lVal == 0 || rVal == 0)
                {
                    visLeft = -1;
                    visRight = -1;
                }

                if (maxLeft < lVal)
                {
                    maxLeft = lVal;
                    if (visLeft != -1)
                        visLeft++;
                }

                if (maxRight < rVal)
                {
                    maxRight = rVal;
                    if (visRight != -1)
                        visRight++;
                }
            }

            return (clueLeft == 0 || visLeft == -1 || visLeft == clueLeft) && (clueRight == 0 || visRight == -1 || visRight == clueRight);
        }

        public override (int i, int j) Coordinates(int i)
        {
            return (i, _startPosition);
        }
    }
}
