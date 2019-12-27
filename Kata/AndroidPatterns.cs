using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class AndroidPatterns
    {
        public int CountPatternsFrom(char firstDot, int length)
        {
            if (length < 1 || length > 9) return 0;
            if (length == 1) return 1;

            int Count(char[] path) => path.Length == length
                ? 1
                : PossibleNext(path).Sum(c => Count(path.Concat(new[] {c}).ToArray()));

            return Count(new[] {firstDot});
        }

        private char[] PossibleNext(char[] path)
        {
            var used = new HashSet<char>(path);
            var possible = new HashSet<char>("ABCDEFGHI".ToCharArray());
            possible.ExceptWith(path);

            switch (path.Last())
            {
                case 'A':
                    if (!used.Contains('E')) possible.Remove('I');
                    if (!used.Contains('D')) possible.Remove('G');
                    if (!used.Contains('B')) possible.Remove('C');
                    break;
                case 'B':
                    if (!used.Contains('E')) possible.Remove('H');
                    break;
                case 'C':
                    if (!used.Contains('E')) possible.Remove('G');
                    if (!used.Contains('B')) possible.Remove('A');
                    if (!used.Contains('F')) possible.Remove('I');
                    break;
                case 'D':
                    if (!used.Contains('E')) possible.Remove('F');
                    break;
                case 'E':
                    break;
                case 'F':
                    if (!used.Contains('E')) possible.Remove('D');
                    break;
                case 'G':
                    if (!used.Contains('E')) possible.Remove('C');
                    if (!used.Contains('D')) possible.Remove('A');
                    if (!used.Contains('H')) possible.Remove('I');
                    break;
                case 'H':
                    if (!used.Contains('E')) possible.Remove('B');
                    break;
                case 'I':
                    if (!used.Contains('E')) possible.Remove('A');
                    if (!used.Contains('H')) possible.Remove('G');
                    if (!used.Contains('F')) possible.Remove('C');
                    break;
            }

            return possible.ToArray();
        }
    }
}