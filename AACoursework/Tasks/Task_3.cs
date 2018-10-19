using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;

namespace AACoursework.Tasks
{
    public static class Task_3
    {
        private static string Replacer(string what, string with, int postition)
        {
            what = what.Remove(postition, 1).Insert(postition, with);
            return what;
        }

        private static void GenerateGrayLikeSequence(Dictionary<int, string> elements, int maxRankInWord, int filterElements, ConcurrentQueue<string> queue)
        {
            var currentWords = new List<string>();
            var firstRankWords = new List<string>();
            var zeroElement = elements[0];
            var ptrToWord = Enumerable.Repeat(zeroElement, maxRankInWord).ToArray();
            currentWords.Add(string.Join("", ptrToWord));
            firstRankWords.Add(currentWords.ElementAt(0));

            for (int i = maxRankInWord - 1; i >= 0; i--)
            {
                for (int j = 1; j < elements.Count; j += 1, firstRankWords.Reverse())
                {
                    var anotherHalf = firstRankWords.Select(x => Replacer(x, elements[j], i)).ToList();
                    currentWords.AddRange(anotherHalf);
                }
                firstRankWords = currentWords.ToList();
                firstRankWords.Reverse();
            }

            var filtered = currentWords.Where(x => Regex.Matches(x, zeroElement).Count >= filterElements);

            foreach (var item in filtered)
            {
                queue.Enqueue(item);
            }
        }

        public static string GenerateGraySequenceEntryQueued(string[] uniqElements, int maxRank,int maxElementInItem)
        {
            var queue = new ConcurrentQueue<string>();
            bool finished = false;

            var sequence = uniqElements.Select((z, index) => new { index = index, letter = z }).ToDictionary(g => g.index, g => g.letter);

            var subsetGenerationTask = Task.Run(() =>
            {
                try
                {
                    GenerateGrayLikeSequence(sequence, maxRank, maxElementInItem, queue);
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

            var fileName = "task_3_results_" + DateTime.Now.ToString("hh_mm_ss");
            using (var fileStream = new StreamWriter(fileName, false, System.Text.Encoding.Unicode))
            {
                fileStream.WriteLine("Used set of elements: " + string.Join(",", sequence));
                fileStream.WriteLine();

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