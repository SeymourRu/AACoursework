using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.IO;

namespace AACoursework.Tasks
{
    public static class Task_4
    {
        private static void AlgorithmH(int n, int m, ConcurrentQueue<string> queue)
        {
            var a = new int[m + 1];
            var x = 0;

            H1:
            a[0] = n - m + 1;
            for (int j1 = 1; j1 < m; j1++) //!
            {
                a[j1] = 1;
            }
            a[m] = -1;

            H2:
            var currentResult = string.Join("", a.Take(a.Length - 1));
            queue.Enqueue(currentResult);
            var a2 = a[1];
            var a1 = a[0];
            if (a2 >= a1 - 1)
            {
                goto H4;
            }

            H3:
            a[0] = a[0] - 1;
            a[1] = a[1] + 1;
            goto H2;

            H4:
            var j = 2;
            var s = a[0] + a[1] - 1;

            if (a[j] >= a[0] - 1)
            {
                do
                {
                    s = s + a[j];
                    j = j + 1;
                }
                while (a[j] >= a[0] - 1);
            }

            H5:

            if ((j + 1) > m)
            {
                return;
            }
            else
            {
                x = a[j] + 1;
                a[j] = x;
                j = j - 1;
            }

            H6:
            while (j > 0)
            {
                a[j] = x;
                s = s - x;
                j = j - 1;
            }

            a[0] = s;
            goto H2;
        }

        private static void AlgorithmHModified(int n, int m, ConcurrentQueue<string> queue)
        {
            var a = new int[m + 1];
            var x = 0;

            H1:
            a[0] = n;
            for (int j1 = 1; j1 < m; j1++) //!
            {
                a[j1] = 0;
            }
            a[m] = -1;

            H2:
            var currentResult = string.Join("", a.Take(a.Length - 1));
            queue.Enqueue(currentResult);
            var a2 = a[1];
            var a1 = a[0];
            if (a2 >= a1 - 1)
            {
                goto H4;
            }

            H3:
            a[0] = a[0] - 1;
            a[1] = a[1] + 1;
            goto H2;

            H4:
            var j = 2;
            var s = a[0] + a[1] - 1;

            if (a[j] >= a[0] - 1)
            {
                do
                {
                    s = s + a[j];
                    j = j + 1;
                }
                while (a[j] >= a[0] - 1);
            }

            H5:

            if ((j + 1) > m)
            {
                return;
            }
            else
            {
                x = a[j] + 1;
                a[j] = x;
                j = j - 1;
            }

            H6:
            while (j > 0)
            {
                a[j] = x;
                s = s - x;
                j = j - 1;
            }

            a[0] = s;
            goto H2;
        }

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

        public static string AlgorithmHEntryQueued(int n, int m, bool useModifiedVersion)
        {
            var queue = new ConcurrentQueue<string>();
            bool finished = false;

            var subsetGenerationTask = Task.Run(() =>
            {
                try
                {
                    if (useModifiedVersion)
                    {
                        //MessageBox.Show("Well ok, but yet it is not implemented !");
                        AlgorithmHModified(n, m, queue);
                    }
                    else
                    {
                        AlgorithmH(n, m, queue);
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

            var fileName = "Task_4_results_" + DateTime.Now.ToString("hh_mm_ss");
            using (var fileStream = new StreamWriter(fileName, false, System.Text.Encoding.ASCII))
            {

                while (true)
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

                    if (finished && queue.IsEmpty)
                    {
                        break;
                    }
                }
            }

            return "See results in " + fileName;
        }
    }
}