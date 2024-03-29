﻿namespace MyThreadPoolTests;

using NUnit.Framework;
using System;
using System.Threading;
using System.Collections.Concurrent;
using MyThreadPool;

/// <summary>
/// Test ThreadSafe of the TPL.
/// </summary>
public class MultiThreadTests
{
    
    private MyThreadPool? threadPool;
    private ManualResetEvent manualResetEvent = new (false);
    private ConcurrentQueue<int>? results;

    private const int numberOfThreads = 2;
    private Thread[] threads = new Thread[numberOfThreads];

    [SetUp]
    public void SetUp()
    {
        threadPool = new MyThreadPool(Environment.ProcessorCount);
        manualResetEvent = new ManualResetEvent(false);
        results = new ConcurrentQueue<int>();
        threads = new Thread[numberOfThreads];
    }

    [Test]
    public void TestParallelThread()
    {
        int tasks = 100000;
        int numberOfThreadsOutThreadPool = 10;
        var task = () => 0;
        var threads = new Thread[numberOfThreadsOutThreadPool];
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(threadPool);
        for (int i = 0; i < numberOfThreadsOutThreadPool; ++i)
        {
            threads[i] = new Thread(() =>
            {
                for (int i = 0; i < tasks / numberOfThreadsOutThreadPool; ++i)
                {
                    var taskTakenByThreadPool = threadPool.Add(task);
                    results.Enqueue(taskTakenByThreadPool.Result);
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.AreEqual(tasks, results.Count);
        foreach (var number in results)
        {
            Assert.AreEqual(0, number);
        }
    }

    [Test]
    public void TestResultIsThreadSafe()
    {
        ArgumentNullException.ThrowIfNull(threadPool);
        ArgumentNullException.ThrowIfNull(results);
        var task = threadPool.Add(() =>
        {
            manualResetEvent.WaitOne();
            return 1;
        });

        for (var i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() => results.Enqueue(task.Result));
            threads[i].Start();
        }

        manualResetEvent.Set();
        foreach (var thread in threads)
        {
            if (!thread.Join(100))
            {
                Assert.Fail();
            }
        }

        Assert.AreEqual(numberOfThreads, results.Count);
        foreach (var result in results)
        {
            Assert.AreEqual(1, result);
        }
    }

    [Test]
    public void TestAddTasksIsThreadSafe()
    {
        ArgumentNullException.ThrowIfNull(threadPool);
        ArgumentNullException.ThrowIfNull(results);
        for (var i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() => results.Enqueue(threadPool.Add(() => 10).Result));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            if (!thread.Join(100))
            {
                Assert.Fail();
            }
        }

        Assert.AreEqual(numberOfThreads, results.Count);
        foreach (var result in results)
        {
            Assert.AreEqual(10, result);
        }
    }
}