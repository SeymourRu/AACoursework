using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace AACoursework.Tasks
{
    public static class Task_2
    {
        private static double MakeCalculation(char op, double left, double right)
        {
            if (op == '+') return left + right;
            if (op == '-') return left - right;
            if (op == '*') return left * right;
            if (op == '/') return left / right;
            return 0.0;
        }

        public static string ProcessAlphametricEntry(string text)
        {
            var letters = text.Where(c => c >= 'A' && c <= 'Z').Distinct().OrderBy(c => c).ToArray();

            var notZero = letters.Select((z, index) => new { letter = z, index = index })
                .Where(x => Regex.IsMatch(text, "[^A-Z]" + x.letter + "|^" + x.letter))
                .Select(x => x.index)
                .ToList();

            var combos = Combinatorics.GetPermutations("0123456789".ToArray(), letters.Length)
                .Where(t => notZero.All(z => t[z] != '0')).ToArray();

            var finalAnswer = "";
            var replacedResult = "";
            var found = false;
            var supportedOperations = new char[] { '+', '-', '*', '/' };
            var sides = text.Split(new string[] { "==", "=" }, StringSplitOptions.None);

            var operationsLeft = new Queue<char>();
            var operationsRight = new Queue<char>();

            foreach (var anotherChar in sides[0])
            {
                if (supportedOperations.Contains(anotherChar))
                {
                    operationsLeft.Enqueue(anotherChar);
                }
            }

            foreach (var anotherChar in sides[1])
            {
                if (supportedOperations.Contains(anotherChar))
                {
                    operationsRight.Enqueue(anotherChar);
                }
            }

            for (var i = 0; i < combos.Length; i++)
            {
                replacedResult = text;
                for (int c = 0; c < letters.Length; c++)
                {
                    replacedResult = replacedResult.Replace(letters[c], combos[i][c]);
                }

                sides = replacedResult.Split(new string[] { "==", "=" }, StringSplitOptions.None);

                var solvedLeft = sides[0].Split(supportedOperations).Select(a => double.Parse(a)).ToList();
                var solvedRight = sides[1].Split(supportedOperations).Select(a => double.Parse(a)).ToList();

                while (solvedLeft.Count > 1)
                {
                    var operation = operationsLeft.Dequeue();
                    var leftOperand = solvedLeft[0];
                    var rightOperand = solvedLeft[1];

                    solvedLeft[0] = MakeCalculation(operation, leftOperand, rightOperand); ;
                    solvedLeft.RemoveAt(1);
                    operationsLeft.Enqueue(operation);
                }

                while (solvedRight.Count > 1)
                {
                    var operation = operationsRight.Dequeue();
                    var leftOperand = solvedRight[0];
                    var rightOperand = solvedRight[1];

                    solvedRight[0] = MakeCalculation(operation, leftOperand, rightOperand);
                    solvedRight.RemoveAt(1);
                    operationsRight.Enqueue(operation);
                }

                found = solvedLeft[0] == solvedRight[0];

                if (found)
                {
                    finalAnswer += "{" + string.Join(", ", letters.ToList().Select((c, index) => "\"" + c + "\"=>" + combos[i][index])) + "}\r\n" + replacedResult;
                }
            }

            return finalAnswer;
        }

        public static string ProcessAlphametricEntryQueued(string text)
        {
            var letters = text.Where(c => c >= 'A' && c <= 'Z').Distinct().OrderBy(c => c).ToArray();
            var currentLetters = new String(letters);

            var notZero = letters.Select((z, index) => new { letter = z, index = index })
                .Where(x => Regex.IsMatch(text, "[^A-Z]" + x.letter + "|^" + x.letter))
                .Select(x => x.index)
                .ToList();

            var queue = new ConcurrentQueue<string>();
            bool finished = false;
            var finalAnswer = "";
            var replacedResult = "";
            var found = false;
            var supportedOperations = new char[] { '+', '-', '*', '/' };
            var sides = text.Split(new string[] { "==", "=" }, StringSplitOptions.None);

            var operationsLeft = new Queue<char>();
            var operationsRight = new Queue<char>();

            foreach (var anotherChar in sides[0])
            {
                if (supportedOperations.Contains(anotherChar))
                {
                    operationsLeft.Enqueue(anotherChar);
                }
            }

            foreach (var anotherChar in sides[1])
            {
                if (supportedOperations.Contains(anotherChar))
                {
                    operationsRight.Enqueue(anotherChar);
                }
            }

            var permutationGenerationTask = Task.Run(() =>
            {
                try
                {
                    Combinatorics.GetPermutationsQueued("0123456789".ToArray(), queue);
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

            while (true)
            {
                while (!queue.IsEmpty)
                {
                    string probableSolution;

                    if (!queue.TryDequeue(out probableSolution))
                    {
                        break;
                    }

                    var badCombination = false;
                    foreach (var nonZeroItem in notZero)
                    {
                        if (probableSolution[nonZeroItem] == '0')
                        {
                            badCombination = true;
                            break;
                        }
                    }

                    if (badCombination)
                    {
                        continue;
                    }

                    replacedResult = text;

                    for (int c = 0; c < letters.Length; c++)
                    {
                        replacedResult = replacedResult.Replace(currentLetters[c], probableSolution[c]);
                    }

                    sides = replacedResult.Split(new string[] { "==", "=" }, StringSplitOptions.None);

                    try
                    {
                        var solvedLeftTest = sides[0].Split(supportedOperations).Select(a => double.Parse(a)).ToList();
                        var solvedRightTest = sides[1].Split(supportedOperations).Select(a => double.Parse(a)).ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }

                    var solvedLeft = sides[0].Split(supportedOperations).Select(a => double.Parse(a)).ToList();
                    var solvedRight = sides[1].Split(supportedOperations).Select(a => double.Parse(a)).ToList();

                    while (solvedLeft.Count > 1)
                    {
                        var operation = operationsLeft.Dequeue();
                        var leftOperand = solvedLeft[0];
                        var rightOperand = solvedLeft[1];

                        solvedLeft[0] = MakeCalculation(operation, leftOperand, rightOperand);
                        solvedLeft.RemoveAt(1);
                        operationsLeft.Enqueue(operation);
                    }

                    while (solvedRight.Count > 1)
                    {
                        var operation = operationsRight.Dequeue();
                        var leftOperand = solvedRight[0];
                        var rightOperand = solvedRight[1];

                        solvedRight[0] = MakeCalculation(operation, leftOperand, rightOperand);
                        solvedRight.RemoveAt(1);
                        operationsRight.Enqueue(operation);
                    }

                    found = solvedLeft[0] == solvedRight[0];

                    if (found)
                    {
                        finalAnswer += "{" + string.Join(", ", letters.ToList().Select((c, index) => "\"" + c + "\"=>" + probableSolution[index])) + "} \r\n" + replacedResult;
                    }
                }

                if (finished && queue.IsEmpty)
                {
                    break;
                }
            }

            return finalAnswer;
        }
    }
}