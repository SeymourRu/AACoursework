using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.IO;

namespace AACoursework.Tasks
{
    public static class Task_β
    {
        private static string Replacer(string what, string with, int postition)
        {
            what = what.Remove(postition, 1).Insert(postition, with);
            return what;
        }

        public static string GenerateSubsetsEntryQueued(string[] values, string[] coefsStr)
        {
            var queue = new ConcurrentQueue<string>();
            bool finished = false;

            var elements = values.Select(x => Int32.Parse(x.ToString())).ToList();
            var coefs = coefsStr.Select(x => Int32.Parse(x.ToString())).ToList();

            var subsetGenerationTask = Task.Run(() =>
            {
                try
                {
                    TopologicalSort(elements, coefs, queue);
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

            var fileName = "Task_beta_results_" + DateTime.Now.ToString("hh_mm_ss");
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

        public static void TopologicalSort(List<int> values, List<int> coefs, ConcurrentQueue<string> queue)
        {
            int j = 0;
            V1:
            V2:
            var currentResult = string.Join("", a.Take(a.Length - 1));
            queue.Enqueue(currentResult);
            V3:
            V4:
            V5:

        }
    }
}