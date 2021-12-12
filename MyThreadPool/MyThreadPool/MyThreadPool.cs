using System.Collections.Concurrent;
namespace MyThreadPool;
public class MyThreadPool
{
    public int NumberOfThreads { get; set; }
    private readonly BlockingCollection<Action> actions = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private object locker = new object();

    public MyThreadPool(int numberOfThreads)
    {
        NumberOfThreads = numberOfThreads;
        var threads = new Thread[numberOfThreads];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() => ExecuteTasks(cancellationTokenSource.Token));
            threads[i].Start();
        }
    }
    public void ShotDown()
    {
        cancellationTokenSource.Cancel();
        lock (locker) { }
    }
    public IMyTask<T> Add<T>(Func<T> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        if (cancellationTokenSource.IsCancellationRequested)
        {
            throw new InvalidOperationException("Cancellation was requested before you added this task");
        }
        var task = new MyTask<T>(func, this);
        try
        {
            actions.Add(task.Run, cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException("Cancellation was requested before you added this task");
        }
        return task;
    }
    private void ExecuteTasks(CancellationToken cancellationToken)
    {

    }
}