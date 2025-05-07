using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Тестові дані
        int[] mainArray = Enumerable.Repeat(new[] { 1, 2, 3, 4, 5, 6 }, 100000).SelectMany(x => x).ToArray();
        int[] pattern = new int[] { 3, 4, 5 };

        // Послідовна версія
        var sw = Stopwatch.StartNew();
        int countSequential = CountPatternSequential(mainArray, pattern);
        sw.Stop();
        Console.WriteLine($"[Послідовно] Кількість входжень: {countSequential}, Час: {sw.ElapsedMilliseconds} мс");

        // Паралельна версія
        sw.Restart();
        int countParallel = CountPatternParallel(mainArray, pattern);
        sw.Stop();
        Console.WriteLine($"[Паралельно] Кількість входжень: {countParallel}, Час: {sw.ElapsedMilliseconds} мс");
    }

    // Послідовна версія
    static int CountPatternSequential(int[] array, int[] pattern)
    {
        int count = 0;
        int n = array.Length;
        int m = pattern.Length;

        for (int i = 0; i <= n - m; i++)
        {
            bool match = true;
            for (int j = 0; j < m; j++)
            {
                if (array[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) count++;
        }

        return count;
    }

    // Паралельна версія
    static int CountPatternParallel(int[] array, int[] pattern)
    {
        int count = 0;
        int n = array.Length;
        int m = pattern.Length;
        object locker = new object();

        Parallel.For(0, n - m + 1, () => 0, (i, state, localCount) =>
        {
            bool match = true;
            for (int j = 0; j < m; j++)
            {
                if (array[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) localCount++;
            return localCount;
        },
        localCount =>
        {
            lock (locker)
            {
                count += localCount;
            }
        });

        return count;
    }
}

