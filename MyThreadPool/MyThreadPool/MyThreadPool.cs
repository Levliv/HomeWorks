using System.Collections.Concurrent;
namespace MyThreadPool;
public class MyThreadPool
{
    public int NumberOfThreads { get; set; }
    private BlockingCollection<Action>? actions = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private object locker = new object();
    private int ShutedDwonThreads = 0;
    private AutoResetEvent autoResetEvent = new(false);
    internal class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; set; } = false;
        public AggregateException? AggregateException { get; set; } = null;
        private TResult result;
        private Func<TResult> func;
        private readonly ManualResetEvent manualResetEvent = new(false);
        private readonly Queue<Action> continueWithTasks = new();
        private MyThreadPool myThreadPool;
        private object locker = new();
        public TResult Result
        {
            get
            {
                manualResetEvent.WaitOne();
                if (AggregateException == null)
                {
                    return result;
                }
                throw AggregateException;
            }
        }
        public MyTask(Func<TResult> func, MyThreadPool myThreadPool)
        {
            this.myThreadPool = myThreadPool;
            this.func = func;

        }
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            TNewResult Func() => func(Result);
            var task = new MyTask<TNewResult>(Func, myThreadPool);
            lock (locker)
            {
                if (IsCompleted)
                {
                    return myThreadPool.Add(Func);
                }
                continueWithTasks.Enqueue(task.Run);
                return task;
            }

        }
        public void Run()
        {
            try
            {

            }
            catch (Exception ex)
            {
                AggregateException = new(ex);
            }
            finally
            {
                lock (locker)
                {
                    while (continueWithTasks.Count > 0)
                    {
                        continueWithTasks.Dequeue()();
                    }
                    IsCompleted = true;
                    func = null;
                    manualResetEvent.Set();

                }
            }
        }
    }

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
        lock (locker) 
        {
            actions?.CompleteAdding();
            while (NumberOfThreads != ShutedDwonThreads)
            {
                autoResetEvent.WaitOne();
            }
            actions = null;
        }
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
            actions?.Add(task.Run, cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException("Cancellation was requested before you added this task");
        }
        return task;
    }
    private void ExecuteTasks(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                autoResetEvent.Set();
                Interlocked.Increment(ref ShutedDwonThreads);
                break;
            }
            else
            {
                Action taskRun = null;
                try
                {
                    taskRun = actions.Take(cancellationToken);

                }
                catch (OperationCanceledException ex)
                {
                    throw ex.InnerException;
                }
            }
        }
    }
}