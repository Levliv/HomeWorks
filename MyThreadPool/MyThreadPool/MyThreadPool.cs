using System.Collections.Concurrent;
namespace MyThreadPool;
public class MyThreadPool
{
    private int NumberOfThreads;
    private ConcurrentQueue<Action>? actions = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private object lockerQueue = new object();
    private AutoResetEvent taskAutoResetEvent = new(false);
    private AutoResetEvent shutDownResetEvent = new(false); 
    private int threadCounter;
    private CancellationToken token;
    public MyThreadPool(int numberOfThreads)
    {
        var token = cancellationTokenSource.Token;
        if (numberOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("Number of threads must be positive");
        }
        NumberOfThreads = numberOfThreads;
        var threads = new Thread[numberOfThreads];
        for (var i = 0; i < threads.Length; i++)
        { 
            threads[i] = new Thread(() => 
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        shutDownResetEvent.Set();
                        Interlocked.Decrement(ref NumberOfThreads);
                        break;
                    }
                    if (actions.TryDequeue(out Action action))
                    {
                        action();
                    }
                }
            });
            threads[i].Start();
        }

    }

    public void ShutDown()
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("Shut 1");
        lock (lockerQueue)
        {
            Console.WriteLine("Shut 2");
            while (threadCounter > 0)
            {
                Console.WriteLine($"Shut thread {threadCounter}");
                shutDownResetEvent.WaitOne();
            }
            Interlocked.Decrement(ref threadCounter);
        }
    }

    public IMyTask<T> Add<T>(Func<T> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        lock (lockerQueue) 
        {
            if (cancellationTokenSource.IsCancellationRequested)
            { 
                throw new InvalidOperationException("Cancellation was requested before you added this task");
            }
            var task = new MyTask<T>(func, this);
            try
            {
                actions.Enqueue(task.RunTask);
                //taskAutoResetEvent.Set();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return task;
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

        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            myThreadPool = threadPool;
            this.func = func;

        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            TNewResult Func() => func(Result);
            var task = new MyTask<TNewResult>(Func, myThreadPool);
            lock (locker)
            {
                if (IsCompleted)
                {
                    return myThreadPool.Add(Func);
                }
                continueWithTasks.Enqueue(task.RunTask);
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
                        if (continueWithTasks.TryDequeue(out Action instance))
                        {
                            myThreadPool.Add(() => instance);
                        } 
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(continueWithTasks));
                        }
                    }
                    IsCompleted = true;
                    func = null;
                    manualReset.Set();
                }
            }
        }
    }
}