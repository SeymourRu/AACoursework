using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Collections;
using System.IO;

namespace AACoursework.Tasks
{
    public static class Task_5
    {
        private static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }

        private static void GetPermutation(char[] list, int k, int m, ConcurrentQueue<string> queue)
        {
            if (k == m)
            {
                queue.Enqueue(new string(list));
            }
            else
                for (int i = k; i <= m; i++)
                {
                    Swap(ref list[k], ref list[i]);
                    GetPermutation(list, k + 1, m, queue);
                    Swap(ref list[k], ref list[i]);
                }
        }

        private static void GetPermutations(char[] list, ConcurrentQueue<string> queue)
        {
            int x = list.Length - 1;
            GetPermutation(list, 0, x, queue);
        }

        private static bool NextCombination(IList<int> num, int n, int k)
        {
            if (k <= 0) return false;

            bool finished;
            var changed = finished = false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;
                    if (i < k - 1)
                    {
                        for (var j = i + 1; j < k; j++)
                        {
                            num[j] = num[j - 1] + 1;
                        }
                    }
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
            {
                numbers[i] = i;
            }

            do
            {
                yield return numbers.Select(n => elem[n]);
            } while (NextCombination(numbers, size, k));
        }

        public static string GenerateSubsetsEntryQueued(string[] values)
        {
            var queue = new ConcurrentQueue<string>();
            bool finished = false;

            var subsetGenerationTask = Task.Run(() =>
            {
                try
                {
                    for (var i = 1; i <= values.Length; i++)
                    {
                        foreach (IEnumerable<string> anotherCombination in Combinations(values, i))
                        {
                            GetPermutations(string.Join("", anotherCombination).ToArray(), queue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    finished = true;
                }
            });

            var fileName = "Task_5_results_" + DateTime.Now.ToString("hh_mm_ss");
            using (var fileStream = new StreamWriter(fileName, false, System.Text.Encoding.ASCII))
            {
                while (!finished)
                {
                    while (!queue.IsEmpty)
                    {
                        string probableSolution;

                        if (!queue.TryDequeue(out probableSolution))
                        {
                            continue;
                        }

                        fileStream.WriteLine(probableSolution);
                    }
                }
            }

            return "See results in " + fileName;
        }
    }
}