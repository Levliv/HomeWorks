using System.Collections.Concurrent;

namespace MyThreadPool;

/// <summary>
/// MyTPL
/// </summary>
public class MyThreadPool
{
    private int NumberOfThreads;
    private Queue<Action>? actions = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private object locker = new object();
    private AutoResetEvent shutDownResetEvent = new(false);
    private CancellationToken token;

    /// <summary>
    /// Returns number of active threads
    /// </summary>
    public int ActiveThreads => NumberOfThreads;

    /// <summary>
    /// Constructor for thread pool
    /// </summary>
    /// <param name="numberOfThreads">Number of threads to start</param>
    /// <exception cref="ArgumentOutOfRangeException"> Exception throws if the number of threads is negative</exception>
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

    /// <summary>
    /// Shuts threads pool down after finishing counting the processed tasks
    /// </summary>
    public void ShutDown()
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("Shut 1");
        lock (locker)
        {
            Console.WriteLine("Shut 2");
            while (NumberOfThreads > 0)
            {
                Console.WriteLine($"Shut thread {NumberOfThreads}");
                shutDownResetEvent.WaitOne();
            }
            Interlocked.Decrement(ref NumberOfThreads);
        }
    }

    /// <summary>
    /// Adding tasks in TaskQueue
    /// </summary>
    /// <exception cref="ArgumentNullException"> Throws if the function is null </exception>
    /// <exception cref="InvalidOperationException"> Throws if cancellation was requested before adding a task </exception>
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
                actions.Enqueue(task.RunTask);
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
        private readonly Queue<Action> continueWithTasks = new();
        private MyThreadPool myThreadPool;
        private object locker = new();
        private ManualResetEvent manualReset = new(false);

        /// <summary>
        /// Contains the information whether the task is completed
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Contains an exception in case it has occured while Running the task
        /// </summary>
        public AggregateException? AggregateException { get; set; } = null;

        /// <summary>
        /// Contains the result of the task Excecution or throws AggregateException if case it has occured
        /// </summary>
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

        /// <summary>
        /// Constructor for a MyTask instance
        /// </summary>
        /// <exception cref="ArgumentNullException">throws in case func is null </exception>
        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            myThreadPool = threadPool;
            this.func = func;

        }

        /// <summary>
        /// Adding ContinueWith tasks in threadPool
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            var task = new MyTask<TNewResult>(() => func(Result), myThreadPool);
            lock (locker)
            {
                if (IsCompleted)
                {
                    Console.WriteLine("Completed");
                    return myThreadPool.Add(() => func(Result));
                }
                Console.WriteLine("There");
                continueWithTasks.Enqueue(task.RunTask);
                return task;
            }
        }

        /// <summary>
        /// Runs a task from the taskQueue
        /// </summary>
        /// <exception cref="ArgumentException"> Throws in case shut down was requested before task was run </exception>
        public void RunTask()
        {
            if (myThreadPool.token.IsCancellationRequested)
            {
                throw new ArgumentException("TreadPool was shuted down before you added this task");
            }
            try
            {
                Console.WriteLine("try");
                result = func();
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
                        Console.WriteLine($"Counter : {continueWithTasks.Count}");
                        myThreadPool.actions.Enqueue(continueWithTasks.Dequeue());
                    }
                    IsCompleted = true;
                    func = null;
                    manualReset.Set();
                }
            }
        }
    }
}