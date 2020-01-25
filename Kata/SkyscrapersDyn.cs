﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kata
{
    public class SkyscrapersDyn
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            return new Board(clues).Solve();
        }
    }

    internal class Board
    {
        public const string AllPossibilities = "123456";

        private readonly string[] _board;
        private readonly bool[] _processed;

        private Board()
        {
            _board = _board = new string[AllPossibilities.Length * AllPossibilities.Length];
            _processed = new bool[_board.Length];
        }

        public Board(int[] clues) : this()
        {
            Clues = clues;

            var size = AllPossibilities.Length;
            for (var i = 0; i < _board.Length; ++i)
                _board[i] = AllPossibilities;

            for (var i = 0; i < size; ++i)
            {
                var pLine = _permutations[CluesForLine(i)];
                for (var j = 0; j < size; ++j)
                {
                    var pColumn = _permutations[CluesForColumn(j)];
                    _board[i * size + j] = Intersection(pLine, pColumn, i, j);
                }
            }
        }

        private Board(Board other) : this()
        {
            var result = new Board { Clues = other.Clues };

            for (var i = 0; i < _board.Length; ++i)
                result._board[i] = other._board[i];
        }

        private void SetBoardValues()
        {
            (int position, string possibility) p;
            while ((p = GetPossibilities()) != default)
            {
                var value = _board[p.position]
                    .Intersect(p.possibility)
                    .Aggregate(new StringBuilder(), (sb, ch) => sb.Append(ch), sb => sb.ToString());

                var snapshot = new Snapshot();
                if (value.Length > 1)
                {
                    foreach (var candidateValue in p.possibility)
                        if (CheckCandidate(p.position, candidateValue))
                        {
                            snapshot.TakeSnapshotAt(p.position, _board);

                            if (!SetBoardValue(p.position, candidateValue.ToString()))
                                snapshot.RestoreSnapshot(_board);

                            break;
                        }
                }
                else
                {
                    snapshot.TakeSnapshotAt(p.position, _board);
                    if (!SetBoardValue(p.position, value))
                        snapshot.RestoreSnapshot(_board);
                }
            }
        }

        private bool SetBoardValue(int position, string value)
        {
            _board[position] = value;

            var l = position / AllPossibilities.Length;
            var c = position % AllPossibilities.Length;

            for (var (i, j) = (l * AllPossibilities.Length, c); i < (l + 1) * AllPossibilities.Length; ++i, j+= AllPossibilities.Length)
            {
                if (i != position)
                    _board[i] = _board[i].Replace(value, "");
                if (j != position)
                    _board[j] = _board[j].Replace(value, "");
            }

            return true;
        }

        private bool CheckCandidate(int position, char candidateValue)
        {
            var (l, c) = (position / AllPossibilities.Length, position % AllPossibilities.Length);
            var (cl, cc) = (CluesForLine(l), CluesForColumn(c));
            var pLine = _permutations[cl].Where(p => p[c] == candidateValue);
            var pColumn = _permutations[cc].Where(p => p[l] == candidateValue);

            var ok = true;
            foreach (var p in pLine)
            {
                ok = true;
                for (var i = 0; i < p.Length; ++i)
                    if (!_board[l * AllPossibilities.Length + i].Contains(p[i]))
                    {
                        ok = false;
                        break;
                    }

                if (ok) break;
            }

            if (ok)
                foreach (var p in pColumn)
                {
                    ok = true;
                    for (var i = 0; i < p.Length; ++i)
                        if (!_board[i * AllPossibilities.Length + c].Contains(p[i]))
                        {
                            ok = false;
                            break;
                        }

                    if (ok) break;
                }

            return ok;
        }

        public int[][] Solve()
        {
            SetBoardValues();

            var result = new int[_board.Length / AllPossibilities.Length][];

            for (var i = 0; i < AllPossibilities.Length; ++i)
            {
                result[i] = new int[AllPossibilities.Length];
                for (var j = 0; j < AllPossibilities.Length; ++j)
                    result[i][j] = int.Parse(_board[i * AllPossibilities.Length + j]);
            }

            return result;
        }

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

        protected int[] Clues { get; private set; }

        private readonly Permutations _permutations = new Permutations();

        public (int cl, int cr) CluesForLine(int line)
        {
            var (x, y) = _lineClues[line];
            return (Clues[x], Clues[y]);
        }

        public (int cl, int cr) CluesForColumn(int column)
        {
            var (x, y) = _columnClues[column];
            return (Clues[x], Clues[y]);
        }

        private string Intersection(IEnumerable<string> pLine, IEnumerable<string> pColumn, int line, int column)
        {
            var lineC = pLine.Select(p => p[column]);
            var columnC = pColumn.Select(p => p[line]);

            return lineC.Intersect(columnC)
                .OrderBy(c => c)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c), sb => sb.ToString());
        }

        public Board Clone()
        {
            return new Board(this);
        }

        public (int position, string possibilities) GetPossibilities()
        {
            var minPair = _processed
                .Select((processed, position) => (position, processed, true))
                .Where(p => !p.processed)
                .OrderBy(p => _board[p.position].Length)
                .FirstOrDefault();

            if (minPair != default)
            {
                _processed[minPair.position] = true;
                return (minPair.position, _board[minPair.position]);
            }

            return default;
        }
    }

    internal class Snapshot
    {
        private int _position = -1;
        private string[] _line;
        private string[] _column;

        public void TakeSnapshotAt(int position, string[] board)
        {
            _position = position;
            var size = Board.AllPossibilities.Length;
            var l = position / size;
            var c = position % size;

            _line = new string[size];
            _column = new string[size];

            for (var (i, j, k) = (l * size, c, 0); i < (l + 1) * size; ++i, j += size, ++k)
            {
                if (i != position)
                    _line[k] = board[i];
                if (j != position)
                    _column[k] = board[j];
            }
        }

        public void RestoreSnapshot(string[] board)
        {
            var size = Board.AllPossibilities.Length;
            var l = _position / size;
            var c = _position % size;

            for (var (i, j, k) = (l* size, c, 0); i<(l + 1) * size; ++i, j += size, ++k)
            {
                board[i] = _line[k];
                board[j] = _column[k];
            }
        }
    }

    internal class Permutations
    {
        private readonly Dictionary<(int cl, int cr), List<string>> _permutations = new Dictionary<(int cl, int cr), List<string>>();

        public Permutations()
        {
            var zero = new int[Root.Length];
            for (var i = 0; i <= Root.Length; ++i)
                for (var j = 0; j <= Root.Length; ++j)
                    _permutations.Add((i, j), StartPermutateForClues(zero, i, j).Select(l => l.Encode()).ToList());
        }

        public static int[] Root { get; } = Board.AllPossibilities.Select(c => c - 48).ToArray();

        public IEnumerable<string> this[(int cl, int cr) index] => _permutations[index];

        private static IEnumerable<int[]> StartPermutateForClues(int[] startingValues, int clue1, int clue2)
        {
            var startingList = Root.Except(startingValues).Where(v => v != 0).ToList();

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
    }

    internal static class Extensions
    {
        public static string Encode(this int[] p)
        {
            return p.Aggregate(new StringBuilder(), (sb, v) => sb.Append(v), sb => sb.ToString());
        }
    }
}
