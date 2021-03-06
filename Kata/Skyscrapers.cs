﻿using System.Collections.Generic;
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
        public const int Dimension = 6;
        public static readonly int[] Zero = { 0, 0, 0, 0, 0, 0 };
        public static readonly int[] Root = { 1, 2, 3, 4, 5, 6 };

        public MatrixPuzzle(int[] clues)
        {
            _clues = clues;
        }

        private readonly int[][] _matrix =
        {
            new int[Dimension],
            new int[Dimension],
            new int[Dimension],
            new int[Dimension],
            new int[Dimension],
            new int[Dimension],
        };

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
        //private static readonly (int, int)[] _lineClues =
        //{
        //    (15, 4),
        //    (14, 5),
        //    (13, 6),
        //    (12, 7),
        //};

        //private static readonly (int, int)[] _columnClues =
        //{
        //    (0, 11),
        //    (1, 10),
        //    (2, 9),
        //    (3, 8),
        //};

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
                        clue.matrixVector.SetValues(Zero);
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
        public static int[] MakeClone(this int[] array)
        {
            var copy = new int[array.Length];
            for (var i = 0; i < MatrixPuzzle.Dimension; ++i)
                copy[i] = array[i];

            return copy;
        }

        public static IEnumerable<int[]> StartPermutateForClues(this int[] startingValues, int clue1, int clue2)
        {
            var startingList = MatrixPuzzle.Root.Except(startingValues).Where(v => v != 0).ToList();

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

        public IEnumerable<int> AsEnumerable()
        {
            for (var i = 0; i < Count; ++i)
                yield return GetValue(i);
        }

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

        protected IEnumerable<int> VirtualMatrixLine(int line, int index, int value)
        {
            for (var i = 0; i < _matrix[line].Length; ++i)
                yield return i != index ? _matrix[line][i] : value;
        }

        protected IEnumerable<int> VirtualMatrixColumn(int column, int index, int value)
        {
            for (var i = 0; i < _matrix.Length; ++i)
                yield return i != index ? _matrix[i][column] : value;
        }
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
            // where is the value to be placed?
            var (x, y) = Coordinates(position);

            // matrix needs either an empty (0) or same value present at those coordinates
            if (_matrix[x][y] != 0 && _matrix[x][y] != value)
                return false;
            
            if (_matrix[x][y] == value)
                return true;

            // the new value must be unique on row x and column y if matrix is empty at x,y
            if (_matrix[x][y] == 0)
                if (VirtualMatrixLine(x, y, _matrix[x][y]).Any(v => v == value)
                    || VirtualMatrixColumn(y, x, _matrix[x][y]).Any(v => v == value))
                    return false;

            // visibility related to clues
            var (clueL, clueR) = _mp.CluesForColumn(position);
            if (clueL != 0 || clueR != 0)
            {
                var mc = VirtualMatrixColumn(y, x, value).ToList();
                var lineContainsEmpty = mc.Any(v => v == 0);

                if (!lineContainsEmpty)
                {
                    if (clueL != 0 && mc.CountVisible() != clueL)
                        return false;

                    if (clueR != 0 && mc.CountVisible(true) != clueR)
                        return false;
                }
            }

            return true;
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
            // where is the value to be placed?
            var (x, y) = Coordinates(position);

            // matrix needs either an empty (0) or same value present at those coordinates
            if (_matrix[x][y] != 0 && _matrix[x][y] != value)
                return false;

            if (_matrix[x][y] == value)
                return true;

            // the new value must be unique on row x and column y if matrix is empty at x,y
            if (_matrix[x][y] == 0)
                if (VirtualMatrixLine(x, y, _matrix[x][y]).Any(v => v == value) 
                    || VirtualMatrixColumn(y, x, _matrix[x][y]).Any(v => v == value))
                    return false;

            // visibility related to clues
            var (clueL, clueR) = _mp.CluesForLine(position);
            if (clueL != 0 || clueR != 0)
            {
                var ml = VirtualMatrixLine(x, y, value).ToList();
                var lineContainsEmpty = ml.Any(v => v == 0);

                if (!lineContainsEmpty)
                {
                    if (clueL != 0 && ml.CountVisible() != clueL)
                        return false;

                    if (clueR != 0 && ml.CountVisible(true) != clueR)
                        return false;
                }
            }

            return true;
        }
        public override (int i, int j) Coordinates(int i)
        {
            return (i, _startPosition);
        }
    }
}
