using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kata
{
    class SkyscrapersDyn
    {
        public static int[][] SolvePuzzle(int[] clues)
        {
            return new Board(clues).Solve();
        }
    }

    internal class Board
    {
        public const string AllPossibilities = "1234";

        private readonly string[] _board;

        private Board()
        {
            _board = _board = new string[AllPossibilities.Length * AllPossibilities.Length];
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
                    SetBoardValue(i * size + j, Intersection(pLine, pColumn, i, j));
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
            foreach (var p in GetPossibilities())
                SetBoardValue(p.position, p.possibilities);
        }

        private void SetBoardValue(int position, string possibility)
        {
            var value = _board[position].Select(c => c).Intersect(possibility).ToString();
            var work = new Queue<(int position, string value)>();

            if (value.Length > 1)
            {
                foreach (var candidateValue in possibility)
                    if (CheckCandidate(position, candidateValue))
                    {
                        value = candidateValue.ToString();
                        break;
                    }
            }

            work.Enqueue((position, value));

            while (work.Count != 0)
            {
                (position, value) = work.Dequeue();

                var l = position / AllPossibilities.Length;
                var c = position % AllPossibilities.Length;

                for (var i = l * AllPossibilities.Length; i < (l + 1) * AllPossibilities.Length; ++i)
                {
                    _board[i] = _board[i].Replace(possibility, "");
                    if (_board[i].Length == 1)
                        work.Enqueue((i, _board[i]));
                }

                for (var i = c * AllPossibilities.Length; i < (c + 1) * AllPossibilities.Length; i += AllPossibilities.Length)
                {
                    _board[i] = _board[i].Replace(possibility, "");
                    if (_board[i].Length == 1)
                        work.Enqueue((i, _board[i]));
                }
            }

            _board[position] = possibility;
        }

        private bool CheckCandidate(int position, char candidateValue)
        {
            var (l, c) = (position / AllPossibilities.Length, position % AllPossibilities.Length);
            var (cl, cc) = (CluesForLine(l), CluesForColumn(c));
            var pLine = _permutations[cl].Where(p => p[c] == candidateValue).Select(p => p[c]).ToList();

            for (var i = 0; i < AllPossibilities.Length; ++i)
            {
                if (cc == (0, 0))
                    continue;

                var cp = _permutations[cc].Where(p => p[l + i] == candidateValue).Select(p => p[l+i]);
                var intersection = cp.Intersect(pLine);

                if (!intersection.Any())
                    return false;
            }

            return true;
        }

        public int[][] Solve()
        {
            SetBoardValues();

            var result = new int[_board.Length / AllPossibilities.Length][];
            for (var i = 0; i < AllPossibilities.Length; ++i)
                for (var j = 0; j < AllPossibilities.Length; ++j)
                    result[i][j] = int.Parse(_board[i * AllPossibilities.Length + j]);

            return result;
        }

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
            var lineC = pLine.Select(p => p[column]).Distinct();
            var columnC = pColumn.Select(p => p[line]).Distinct();

            return lineC.Intersect(columnC).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c), sb => sb.ToString());
        }

        public Board Clone()
        {
            return new Board(this);
        }

        public IEnumerable<(int position, string possibilities)> GetPossibilities()
        {
            return _board.Select((value, index) => (index, value)).OrderByDescending(v => v.value.Length);
        }
    }

    internal class PossibilitiesComparer : IComparer<KeyValuePair<int, string>>
    {
        public int Compare(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return y.Value.Length.CompareTo(x.Value.Length);
        }
    }

    internal class Permutations
    {
        private readonly Dictionary<(int cl, int cr), List<string>> _permutations = new Dictionary<(int cl, int cr), List<string>>();

        public Permutations()
        {
            for (var i = 0; i <= Root.Length; ++i)
                for (var j = 0; j <= Root.Length; ++j)
                    _permutations.Add((i, j), StartPermutateForClues(Root, i, j).Select(l => l.Encode()).ToList());
        }

        public static int[] Root { get; } = Board.AllPossibilities.Select(c => c + 48).ToArray();

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
