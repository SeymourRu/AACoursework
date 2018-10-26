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
                        foreach (IEnumerable<string> anotherCombination in Combinatorics.Combinations(values, i))
                        {
                            Combinatorics.GetPermutationsQueued(string.Join("", anotherCombination).ToArray(), queue);
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

    }
}