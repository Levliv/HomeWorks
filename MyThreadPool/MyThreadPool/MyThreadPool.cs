namespace MyThreadPool;

/// <summary>
/// MyTPL.
/// </summary>
public class MyThreadPool
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// Constructor for thread pool.
    /// </summary>
    /// <param name="numberOfThreads">Number of threads to start.</param>
    /// <exception cref="ArgumentOutOfRangeException"> Exception throws if the number of threads is negative.</exception>
    public MyThreadPool(int numberOfThreads)
    {
        var token = cts.Token;
        if (numberOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("Number of threads must be positive");
        }

        this.numberOfThreads = numberOfThreads;
        threads = new Thread[this.numberOfThreads];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (actions.Count == 0)
                    {
                        newTask.WaitOne();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (actions.TryDequeue(out Action? action))
                    {
                        action();
                    }
                    else
                    {
                        throw new AggregateException("Unexpected erroe"); // Надо додумать
                    }
                }
            });
            threads[i].Start();
        }
    }

    private readonly CancellationTokenSource cts = new ();
    private int numberOfThreads;
    private Queue<Action> actions = new ();
    private AutoResetEvent newTask = new (false);
    private Thread[] threads;

    /// <summary>
    /// Gets number of active threads.
    /// </summary>
    public int ActiveThreads => numberOfThreads;

    /// <summary>
    /// Shuts threads pool down after finishing counting the processed tasks.
    /// </summary>
    public void ShutDown()
    {
        lock (actions)
        {
            cts.Cancel();
            for (int i = 0; i < threads.Length; ++i)
            {
                while (actions.Count > 0)
                {
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Adding tasks in TaskQueue.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Throws if the function is null. </exception>
    /// <exception cref="InvalidOperationException"> Throws if cancellation was requested before adding a task. </exception>
    public IMyTask<T> Add<T>(Func<T>? func)
    {
        ArgumentNullException.ThrowIfNull(func);
        lock (actions)
        {
            if (!cts.IsCancellationRequested)
            {
                var task = new MyTask<T>(func, this);
                try
                {
                    if (actions.Count == 0)
                    {
                        newTask.Set();
                    }

                    actions.Enqueue(task.RunTask);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }

                return task;
            }

            throw new InvalidOperationException();
        }
    }

    public class MyTask<TResult> : IMyTask<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// Constructor for a MyTask instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">throws in case func is null. </exception>
        public MyTask(Func<TResult>? func, MyThreadPool threadPool)
        {
            ArgumentNullException.ThrowIfNull(func);
            myThreadPool = threadPool;
            this.func = func;
            continueWithTasks = new();
        }

        private TResult result;
        private Func<TResult>? func;
        private Queue<Action> continueWithTasks;
        private MyThreadPool myThreadPool;
        private ManualResetEvent manualReset = new (false);

        /// <summary>
        /// Contains the information whether the task is completed.
        /// </summary>
        public bool IsCompleted { get; private set; } = false;

        /// <summary>
        /// Contains an exception in case it has occured while Running the task.
        /// </summary>
        public AggregateException? AggregateException { get; set; } = null;

        /// <summary>
        /// Contains the result of the task Excecution or throws AggregateException if case it has occured.
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
        /// Adding ContinueWith tasks in threadPool.
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            var task = new MyTask<TNewResult>(() => func(Result), myThreadPool);
            lock (continueWithTasks)
            {
                lock (myThreadPool.actions)
                {
                    if (IsCompleted)
                    {
                        return myThreadPool.Add(() => func(Result));
                    }

                    continueWithTasks.Enqueue(task.RunTask);
                    return task;
                }
            }
        }

        /// <summary>
        /// Runs a task from the taskQueue.
        /// </summary>
        /// <exception cref="ArgumentException"> Throws in case shut down was requested before task was run. </exception>
        public void RunTask()
        {
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                AggregateException = new(ex);
            }
            finally
            {
                IsCompleted = true;
                manualReset.Set();
                func = null;
                lock (continueWithTasks)
                {
                    while (continueWithTasks.Count > 0)
                    {
                        myThreadPool.Add(() => continueWithTasks.Dequeue());
                    }
                }
            }
        }
    }
}