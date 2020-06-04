using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Skyscrapers
    {
        static int dimension;
        static int[][] possibilities;
        static List<int>[,] countIndices;

        public static int[][] SolvePuzzle(int[] clues)
        {
            dimension = clues.Length / 4;
            countIndices = new List<int>[dimension + 1, dimension + 1];
            List<int[]> permutations = new List<int[]>();

            int[] items = new int[dimension];
            for (int i = 0; i < items.Length; i++) items[i] = i + 1;
            perm(items, dimension, 0, permutations);
            possibilities = permutations.ToArray();

            for (int i = 0; i < possibilities.Length; i++)
            {
                int[] current = possibilities[i];
                int forward = getCount(current);

                int[] reverse = (int[]) current.Clone();
                Array.Reverse(reverse);
                int backward = getCount(reverse);

                if (countIndices[forward, backward] == null)
                    countIndices[forward, backward] = new List<int>();

                countIndices[forward, backward].Add(i);
            }

            List<int>[] possibleRows = new List<int>[dimension];
            List<int>[] possibleCols = new List<int>[dimension];

            analyzeClues(clues, possibleCols, possibleRows);

            int lastCount;
            int count = 2 * dimension * possibilities.Length;

            do
            {
                lastCount = count;
                eliminate(possibleCols, possibleRows);
                eliminate(possibleRows, possibleCols);
                count = getPossibilityCount(possibleCols, possibleRows);

            } while (lastCount != count);

            int[][] result = writeSolution(possibleRows);
            return result;
        }

        private static int getCount(int[] row)
        {
            int count = 0;
            int max = 0;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] > max)
                {
                    count++;
                    max = row[i];
                }
            }

            return count;
        }

        private static void perm(int[] symbols, int n, int i, List<int[]> result)
        {
            if (i >= n - 1) result.Add((int[]) symbols.Clone());
            else
            {
                perm(symbols, n, i + 1, result);
                for (int j = i + 1; j < n; j++)
                {
                    swap(symbols, i, j);
                    perm(symbols, n, i + 1, result);
                    swap(symbols, i, j);
                }
            }
        }

        private static void swap(int[] symbols, int i, int j)
        {
            int tmp = symbols[i];
            symbols[i] = symbols[j];
            symbols[j] = tmp;
        }

        private static void analyzeClues(int[] clues, List<int>[] possibleCols, List<int>[] possibleRows)
        {
            for (int i = 0; i < dimension; i++)
            {
                //find available column combinations
                int fwd = clues[i];
                int rear = clues[3 * dimension - 1 - i];
                possibleCols[i] = getListOfPossible(fwd, rear);
            }

            for (int i = 0; i < dimension; i++)
            {
                //find available row combinations
                int rear = clues[i + dimension];
                int fwd = clues[4 * dimension - 1 - i];
                possibleRows[i] = getListOfPossible(fwd, rear);
            }
        }

        private static List<int> getListOfPossible(int fwd, int rear)
        {
            List<int> result;
            if (fwd != 0 && rear != 0) //both info provided
                result = new List<int>(countIndices[fwd, rear]);
            else if (fwd == 0 && rear == 0) //no info provided
                result = new List<int>(Enumerable.Range(0, possibilities.Length));
            else if (fwd != 0)
            {
                //only front info provided
                result = new List<int>();
                for (int i = 1; i < dimension + 1; i++)
                {
                    if (countIndices[fwd, i] != null) result.AddRange(countIndices[fwd, i]);
                }
            }
            else
            {
                //only rear info provided
                result = new List<int>();
                for (int i = 1; i < dimension + 1; i++)
                {
                    if (countIndices[i, rear] != null) result.AddRange(countIndices[i, rear]);
                }
            }

            return result;
        }

        private static void eliminate(List<int>[] possibleCols, List<int>[] possibleRows)
        {
            for (int col = 0; col < dimension; col++)
            {
                for (int row = 0; row < dimension; row++)
                {
                    bool[] possible = new bool[dimension + 1];
                    foreach (int variantIndex in possibleCols[col])
                    {
                        int[] numbers = possibilities[variantIndex];
                        possible[numbers[row]] = true;
                    }

                    //now I know which numbers are possible in this column and row
                    List<int> toDelete = new List<int>();
                    foreach (int variantIndex in possibleRows[row])
                    {
                        int[] numbers = possibilities[variantIndex];
                        if (!possible[numbers[col]])
                        {
                            toDelete.Add(variantIndex);
                        }
                    }

                    foreach (int variantIndex in toDelete)
                    {
                        possibleRows[row].Remove(variantIndex);
                    }
                }
            }
        }

        private static int getPossibilityCount(List<int>[] possibleCols, List<int>[] possibleRows)
        {
            int count = 0;
            for (int i = 0; i < possibleCols.Length; i++)
                count += possibleCols[i].Count();
            for (int i = 0; i < possibleRows.Length; i++)
                count += possibleRows[i].Count();
            return count;
        }

        private static int[][] writeSolution(List<int>[] possibleRows)
        {
            int[][] result = new int[dimension][];
            for (int i = 0; i < dimension; i++)
            {
                result[i] = possibilities[possibleRows[i][0]];
            }

            return result;
        }
    }
}