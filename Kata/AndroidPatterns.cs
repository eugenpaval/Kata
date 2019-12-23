using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class AndroidPatterns
    {
        //private const string _matrix = "ABCDEFGHI";
        //private readonly Dictionary<char, List<char>> _nonNeighbors = new Dictionary<char, List<char>>
        //{
        //    ['A'] = new List<char> { 'C', 'G', 'I' },
        //    ['B'] = new List<char> { 'H' },
        //    ['C'] = new List<char> { 'A', 'G', 'I' },
        //    ['D'] = new List<char> { 'F' },
        //    ['E'] = new List<char>(),
        //    ['F'] = new List<char> { 'D' },
        //    ['G'] = new List<char> { 'A', 'C', 'I' },
        //    ['H'] = new List<char> { 'B' },
        //    ['I'] = new List<char> { 'A', 'C', 'G' }
        //};
        private const string _matrix = "ABCD";
        private readonly Dictionary<char, List<char>> _nonNeighbors = new Dictionary<char, List<char>>
        {
            ['A'] = new List<char>(),
            ['B'] = new List<char>(),
            ['C'] = new List<char>(),
            ['D'] = new List<char>(),
        };

        //public static int CountPatternsFrom(char firstDot, int length)
        //{
        //    if (length == 0 || length > 9)
        //        return 0;

        //    if (length == 1)
        //        return 1;

        //    return new AndroidPatterns().CountPatterns(firstDot, length);
        //}

        private int CountPatterns(in char dot, in char fromDot, in int length)
        {
            if (length == 2)
                return 8 - _nonNeighbors[dot].Count;

            var counter = 0;
            // DFS(length)
            foreach (var neighbor in GetNeighboursOf(dot)) 
                counter += CountPatterns(neighbor, dot, length - 1);

            return counter;
        }

        private int _counter;
        private readonly HashSet<char> _visited = new HashSet<char>();

        public int Count(in char startNode, in int length)
        {
            TraverseTo(startNode, length);
            return _counter;
        }

        public void TraverseTo(in char startDot, in int length)
        {
            if (length == 1)
            {
                ++_counter;
                return;
            }

            _visited.Add(startDot);
            foreach (var dot in GetNeighboursOf(startDot))
            {
                if (_visited.Contains(dot))
                    continue;

                TraverseTo(dot, length - 1);
            }
        }

        public List<char> GetNeighboursOf(char dot)
        {
            return _matrix.Where(d => d != dot).Except(_nonNeighbors[dot]).ToList();
        }
    }
}
