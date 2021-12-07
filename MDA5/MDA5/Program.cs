using System.Diagnostics;

namespace MDA5;

class Programm
{
    private static (long, double) MethodRunner(Func<byte[]> myMethod)
    {
        var cyrcles = 10;
        var stopWatch = Stopwatch.StartNew();
        var times = new List<long>();
        for (int i = 0; i < cyrcles; i++)
        {
            stopWatch.Restart();
            myMethod();
            stopWatch.Stop();
            times.Add(stopWatch.ElapsedMilliseconds);
        }
        var sum = times.Sum();
        var avg = sum / cyrcles;
        long deviation = 0;
        foreach (var timeInstance in times)
        {
            deviation += (timeInstance - avg) * (timeInstance - avg);
        }
        deviation /= cyrcles;
        var standardDeviationQuadratic = Math.Sqrt(deviation);
        return (avg, standardDeviationQuadratic);
    }
    private static void Timer()
    {
        var path = "..\\..\\..\\..\\Tests";
        for (var i = 0; i < 10; ++i)
        {
            var (singleThreadTime, singleThreadSQD) = MethodRunner(() => HashCounter.ComputeHashSingleThread(path));
            var (MultiThreadTime, MultiThreadSQD) = MethodRunner(() => HashCounter.ComputeHashMultiThread(path));
            Console.WriteLine("Single Thread: {0}ms +- {1}ms", singleThreadTime, singleThreadSQD);
            Console.WriteLine("Multi Thread: {0}ms +- {1}ms", MultiThreadTime, MultiThreadSQD);
        }

    }
    public static void Main(string[] args)
    {
        var path = args[0];
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Directory does not exists");
        }
        Timer();
        Console.WriteLine(BitConverter.ToString(HashCounter.ComputeHashSingleThread(path)));
    }
}
