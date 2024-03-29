﻿namespace MyThreadPool;

using System.Collections.Concurrent;
using System;

/// <summary>
/// My Thread Pool.
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource cts = new ();
    private int numberOfThreads;
    private ConcurrentQueue<Action> actions = new ();
    private AutoResetEvent newTask = new (false);
    private AutoResetEvent shutDown = new (false);
    private int activeThreads;
    private Thread[] threads;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// Constructor for thread pool.
    /// </summary>
    /// <param name="numberOfThreads">Number of threads to start.</param>
    /// <exception cref="ArgumentOutOfRangeException"> Exception throws if the number of threads is negative.</exception>
    public MyThreadPool(int numberOfThreads)
    {
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
                while (true)
                {
                    if (cts.Token.IsCancellationRequested && actions.IsEmpty)
                    {
                        Interlocked.Decrement(ref activeThreads);
                        shutDown.Set();
                        return;
                    }
                    if (actions.TryDequeue(out Action? action))
                    {
                        action();
                    }
                    else
                    {
                        newTask.WaitOne();
                    }
                }
            });
            threads[i].Start();
            Interlocked.Increment(ref activeThreads);
        }
    }

    /// <summary>
    /// Gets number of active threads.
    /// </summary>
    public int TotalNumberOfThreads => numberOfThreads;

    /// <summary>
    /// Shuts threads pool down after finishing counting the processed tasks.
    /// </summary>
    public void ShutDown()
    {
        lock (cts)
        {
            cts.Cancel();
        }

        while (activeThreads > 0)
        {
            newTask.Set();
            shutDown.WaitOne();
        }
    }

    /// <summary>
    /// Adding tasks in TaskQueue.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Throws if the function is null. </exception>
    /// <exception cref="InvalidOperationException"> Throws if cancellation was requested before adding a task. </exception>
    public IMyTask<T> Add<T>(Func<T> func)
    {
        lock (cts)
        {
            if (activeThreads != 0 && !cts.Token.IsCancellationRequested)
            {
                var task = new MyTask<T>(func, this);
                EnqueueTask(task.RunTask);
                return task;
            }

            throw new InvalidOperationException("Thread Pool was shut down");
        }
    }

    private void EnqueueTask(Action task)
    {
        actions.Enqueue(task);
        newTask.Set();
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private TResult? result;
        private Func<TResult>? func;
        private Queue<Action> continueWithTasks = new ();
        private MyThreadPool myThreadPool;
        private ManualResetEvent manualReset = new (false);
        
        /// <summary>
        /// Contains an exception in case it has occured while Running the task.
        /// </summary>
        private AggregateException? gotException { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// Constructor for a MyTask instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">throws in case func is null. </exception>
        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            myThreadPool = threadPool;
            this.func = func;
        }

        /// <summary>
        /// Gets a value indicating whether the task is completed.
        /// </summary>
        public bool IsCompleted { get; private set; } = false;

        /// <summary>
        /// Contains the result of the task Excecution or throws AggregateException if case it has occured.
        /// </summary>
        public TResult Result
        {
            get
            {
                manualReset.WaitOne();
                if (gotException == null)
                {
                    ArgumentNullException.ThrowIfNull(result);
                    return result;
                }

                throw gotException;
            }
        }

        /// <summary>
        /// Adding ContinueWith tasks in threadPool.
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            ArgumentNullException.ThrowIfNull(func);
            lock (myThreadPool.cts)
            {
                if (myThreadPool.cts.Token.IsCancellationRequested)
                {
                    throw new InvalidOperationException("Thread Pool was shut down");
                }

                var task = new MyTask<TNewResult>(() => func(Result), myThreadPool);
                lock (continueWithTasks)
                {
                    if (!IsCompleted)
                    {
                        continueWithTasks.Enqueue(task.RunTask);
                    }
                    else
                    {
                        myThreadPool.EnqueueTask(task.RunTask);
                    }
                    return task;
                }
            }
        }

        /// <summary>
        /// Runs a task from the taskQueue.
        /// </summary>
        /// <exception cref="ArgumentNullException"> Throws in case shut down was requested before task was run. </exception>
        public void RunTask()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(func);
                result = func();
            }
            catch (Exception ex)
            {
                gotException = new AggregateException(ex);
            }
            finally
            {
                func = null;
                lock (continueWithTasks)
                {
                    IsCompleted = true;
                    manualReset.Set();
                    while (continueWithTasks.Count > 0)
                    {
                        myThreadPool.EnqueueTask(continueWithTasks.Dequeue());

                    }
                }
            }
        }
    }
}