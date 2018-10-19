using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AACoursework.Tasks
{
    public static class Task_1_a
    {
        private static void ProcessSubNode(int current, int max, string currentPath, List<KeyValuePair<int, Node<int>>> beams, StreamWriter result)
        {
            if (current < max)
            {
                var item = beams.ElementAt(current);
                var exactCurrentPath = currentPath + item.Key;

                if (item.Value.nodes.Count > 0)
                {
                    WalkThroughTree(item.Value, exactCurrentPath, result);
                }
                else
                {
                    result.WriteLine(exactCurrentPath + item.Value._value);
                }

                ProcessSubNode(current + 1, max, currentPath, beams, result);
            }
        }

        private static void WalkThroughTree(Node<int> node, string currentPath, StreamWriter result)
        {
            var keyValuePair = node.nodes.ToList();

            if (currentPath != "")
            {
                result.WriteLine(currentPath + node._value);
            }

            ProcessSubNode(0, keyValuePair.Count, currentPath, keyValuePair, result);
        }

        private static void ProcessSubNode(int current, int max, string currentPath, List<KeyValuePair<int, Node<int>>> beams, List<string> result)
        {
            if (current < max)
            {
                var item = beams.ElementAt(current);
                var exactCurrentPath = currentPath + item.Key;

                if (item.Value.nodes.Count > 0)
                {
                    WalkThroughTree(item.Value, exactCurrentPath, result);
                }
                else
                {
                    result.Add(exactCurrentPath + item.Value._value);
                }

                ProcessSubNode(current + 1, max, currentPath, beams, result);
            }
        }

        private static void WalkThroughTree(Node<int> node, string currentPath, List<string> result)
        {
            var keyValuePair = node.nodes.ToList();

            if (currentPath != "")
            {
                result.Add(currentPath + node._value);
            }

            ProcessSubNode(0, keyValuePair.Count, currentPath, keyValuePair, result);
        }

        private static void GenerateAdditions(int currentVal, int currentSubstraction, int maxVal, Node<int> node, Dictionary<int, Node<int>> uniqNodes)
        {
            var tempValue = currentVal - currentSubstraction;

            if (tempValue > 0)
            {
                if (currentSubstraction != 0)
                {
                    if (uniqNodes.ContainsKey(tempValue))
                    {
                        node.nodes.Add(currentSubstraction, uniqNodes[tempValue]);
                    }
                    else
                    {
                        var newNode = new Node<int>(tempValue);
                        GenereateNodes(tempValue, maxVal, newNode, uniqNodes);
                        node.nodes.Add(currentSubstraction, newNode);
                        uniqNodes.Add(tempValue, newNode);
                    }

                    GenerateAdditions(currentVal, currentSubstraction - 1, maxVal, node, uniqNodes);
                }
            }
        }

        private static void GenereateNodes(int currentVal, int maxVal, Node<int> node, Dictionary<int, Node<int>> uniqNodes)
        {
            if (currentVal > 0)
            {
                GenerateAdditions(currentVal, currentVal - 1, maxVal, node, uniqNodes);
            }
        }

        private static Node<int> GenereateNodesEntry(int maxVal)
        {
            var headNode = new Node<int>(maxVal);
            var uniqNodes = new Dictionary<int, Node<int>>();
            GenereateNodes(maxVal, maxVal, headNode, uniqNodes);
            return headNode;
        }

        public static void DistributeEntry(int searchingValue, List<string> result)
        {
            
            if (searchingValue > 25)
            {
                MessageBox.Show("Too many nodes will be generated!Be nice and try lower value,ok? (How about <= 25?)");
                result.Add("Too much data :(");
            }
            else if (searchingValue > 10)
            {
                var fileName = "task_1_a_results_" + searchingValue;
                result.Add("See file " + fileName);
                var headNode = GenereateNodesEntry(searchingValue);
                using (var fileStream = new StreamWriter(fileName, false, System.Text.Encoding.ASCII))
                {
                    fileStream.WriteLine(headNode._value.ToString());
                    WalkThroughTree(headNode, "", fileStream);
                }
            }
            else
            {
                var headNode = GenereateNodesEntry(searchingValue);
                result.Add(headNode._value.ToString());
                WalkThroughTree(headNode, "", result);
            }
        }
    }

    public class Node<T>
    {
        public T _value;
        public Dictionary<T, Node<T>> nodes = new Dictionary<T, Node<T>>();

        public Node(T val)
        {
            _value = val;
        }
    }
}