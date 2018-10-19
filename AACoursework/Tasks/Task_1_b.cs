using System.Collections.Generic;

namespace AACoursework.Tasks
{
    public static class Task_1_b
    {
        private static void Distribute(int n, int currentPosition, int maxPosition, List<int> sequence, Dictionary<int, int> result)
        {
            result.Add(currentPosition, n);

            if (currentPosition < maxPosition)
            {
                n = n - sequence[currentPosition];
                Distribute(n, currentPosition + 1, maxPosition, sequence, result);
            }
        }

        public static void DistributeEntry(int n, List<int> sequence, Dictionary<int, int> result)
        {
            Distribute(n, 0, sequence.Count, sequence, result);
        }
    }
}