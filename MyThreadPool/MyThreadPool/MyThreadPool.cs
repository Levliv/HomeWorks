using System.Collections.Concurrent;
namespace MyThreadPool;
public class MyThreadPool
{
    public int NumberOfThreads { get; set; }
    private BlockingCollection<Action>? actions = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private object locker = new object();
    private int ShutedDwonThreads = 0;
    private AutoResetEvent taskAutoResetEvent = new(false);
    private AutoResetEvent shutDownResetEvent = new(false); 
    private int threadCounter;

    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("Number of threads must be positive");
        }
        NumberOfThreads = numberOfThreads;
        var threads = new Thread[numberOfThreads];
        for (var i = 0; i < threads.Length; i++)
        { 
            threads[i] = new Thread(() => ExecuteTasks(cancellationTokenSource.Token));
            threads[i].Start();
            Interlocked.Increment(ref threadCounter);
        }

    }
    public void ShutDown()
    {
        cancellationTokenSource.Cancel();
        lock (locker)
        {
            while (threadCounter > 0)
            {
                shutDownResetEvent.WaitOne();
                Interlocked.Decrement(ref threadCounter);
            }
        }
    }
    public IMyTask<T> Add<T>(Func<T> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        lock (locker) 
        {
            if (cancellationTokenSource.IsCancellationRequested)
            { 
                throw new InvalidOperationException("Cancellation was requested before you added this task");
            }
            var task = new MyTask<T>(func, this);
            try
            {
                actions.Enqueue(task.Run, cancellationTokenSource.Token);
                taskAutoResetEvent.Set();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return task;
        }
    }
    private void ExecuteTasks(CancellationToken cancellationToken)
    {
        while (!(cancellationToken.IsCancellationRequested || actions.IsEmpty)) {
            if (actions.TryDequeue(out Action action))
            {
                action();
            }
            else
            {
                taskAutoResetEvent.WaitOne();
            }
        }
    }



    internal class MyTask<TResult> : IMyTask<TResult>
    {
        private TResult result;
        private Func<TResult> func;
        private readonly ConcurrentQueue<Action> continueWithTasks = new();
        private MyThreadPool myThreadPool;
        private object locker = new();
        private ManualResetEvent manualReset = new(false);
        public bool IsCompleted { get; set; } = false;
        public AggregateException? AggregateException { get; set; } = null;
        public TResult Result
        {
            get
            {
                manualReset.WaitOne();
                if (AggregateException == null)
                {
                    return result;
                }
                throw AggregateException;
            }
        }
        public MyTask(Func<TResult> func, MyThreadPool myThreadPool)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
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
        public void RunTask()
        {
            try
            {
                result = func();
                func = null;
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
                        myThreadPool.Add(() => continueWithTasks.TryDequeue());
                    }
                    IsCompleted = true;
                    func = null;
                    //manualReset.Set();
                }
            }
        }
    }
}