using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreTestingForKatas
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write("Clues> ");
                var clues = Console.ReadLine().Split(new[] {',', ' ', ';'}).Select(s => int.Parse(s)).ToList();
                var result = new [] {0,0,1,0}.StartPermutateForClues(clues[0], clues[1]);
                
                var i = 0;
                foreach (var p in result)
                {
                    Console.Write($"{i++}: ");
                    foreach (var e in p)
                        Console.Write($"{e}");

                    Console.Write("\n");
                }
            }
        }

        //public static IEnumerable<int[]> PermutateForClues(int clue1, int clue2)
        //{
        //    var startingList = new List<int> {1, 2, 3, 4, 5, 6};
        //    IEnumerable<List<int>> result;

        //    if (clue1 != 0 && clue2 != 0)
        //        result = startingList.PermutateForClues(clue1, clue2);
        //    else if (clue1 != 0 && clue2 == 0)
        //        result = startingList.PermutateForClue(clue1);
        //    else if (clue1 == 0 && clue2 != 0)
        //        result = startingList.PermutateForClue(clue2)
        //            .Select
        //            (
        //                l =>
        //                {
        //                    l.Reverse();
        //                    return l;
        //                }
        //            );
        //    else
        //        result = startingList.PermutateForClue();

        //    return result.Select(l => l.ToArray());
        //}
        
    }

    static class Extensions
    {
        public static IEnumerable<int[]> StartPermutateForClues(this int[] startingValues, int clue1, int clue2)
        {
            var startingList = new List<int> { 1, 2, 3, 4 }.Except(startingValues).Where(v => v != 0).ToList();

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

        public static IEnumerable<List<int>> PermutateForClues(this List<int> current, int clue1, int clue2)
        {
            return clue2 == 0
                ? current.PermutateForClue(clue1)
                : current.PermutateForClue(clue1).Where(l => l.CountVisible(true) == clue2);
        }

        public static IEnumerable<List<int>> PermutateForClue(this List<int> current, int clue)
        {
            return clue == 0 
                ? current.PermutateForClue(new List<int>(), clue)
                : current.PermutateForClue(new List<int>(), clue).Where(l => l.CountVisible() == clue);
        }

        public static IEnumerable<List<int>> PermutateForClue(this List<int> current, List<int> previous, int clue)
        {
            if (current.Count == 0)
                yield return current;

            foreach (var e in current)
            {
                var visible = previous.All(x => x < e);
                var newClue = visible ? clue - 1 : clue;

                if (newClue < 0)
                    continue;

                previous.Add(e);

                foreach (var p in current.Where(x => x != e).ToList().PermutateForClue(previous, newClue))
                {
                    var result = new List<int> {e};
                    result.AddRange(p);

                    yield return result;
                }

                previous.Remove(e);
            }
        }

        public static IEnumerable<List<int>> PermutateForClue(this List<int> current)
        {
            if (current.Count == 0)
                yield return current;

            foreach (var e in current)
            {
                foreach (var p in current.Where(x => x != e).ToList().PermutateForClue())
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
}
