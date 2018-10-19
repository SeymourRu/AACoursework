using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;

namespace AACoursework
{
    public static class Combinatorics
    {
        public static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }

        public static char[][] GetPermutations(char[] list, int length)
        {
            if (length == 1) return list.Select(t => new char[] { t }).ToArray();
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)).ToArray(),
                    (t1, t2) => t1.Concat(new char[] { t2 }).ToArray()).ToArray();
        }

        public static void GetPermutationsQueued(char[] list, ConcurrentQueue<string> queue)
        {
            int x = list.Length - 1;
            GetPermutationsQueued(list, 0, x, queue);
        }

        public static void GetPermutationsQueued(char[] list, int k, int m, ConcurrentQueue<string> queue)
        {
            if (k == m)
            {
                queue.Enqueue(new string(list));
            }
            else
            {
                for (int i = k; i <= m; i++)
                {
                    Combinatorics.Swap(ref list[k], ref list[i]);
                    GetPermutationsQueued(list, k + 1, m, queue);
                    Combinatorics.Swap(ref list[k], ref list[i]);
                }
            }
        }

        public static bool NextCombination(IList<int> num, int n, int k)
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

        public static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
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
    }
}
