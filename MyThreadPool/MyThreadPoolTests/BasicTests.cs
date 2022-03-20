namespace MyThreadPoolTests;

using NUnit.Framework;
using System;

/// <summary>
/// Testing TPL with some one thread scenario to catch some mistakes.
/// </summary>
public class BasicTests
{
    private MyThreadPool.MyThreadPool? threadPool;

    [SetUp]
    public void Setup()
    {
        threadPool = new MyThreadPool.MyThreadPool(1);
    }

    [Test]
    public void NegativeThreadsTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool.MyThreadPool(-3));
    }

    [Test]
    public void SimpleTaskComputationTest()
    {
        var task1 = threadPool.Add(() => 1);
        Assert.AreEqual(1, task1.Result);
    }

    [Test]
    public void MultipleTasksComputationTests()
    {
        var task1 = threadPool.Add(() => 1);
        var task2 = threadPool.Add(() => 2);
        var task3 = threadPool.Add(() => 3);
        Assert.AreEqual(1, task1.Result);
        Assert.AreEqual(3, task3.Result);
        Assert.AreEqual(2, task2.Result);
    }

    [Test]
    public void TaskAfterShutDownTest()
    {
        threadPool.ShutDown();
        Assert.Throws<InvalidOperationException>(() => threadPool.Add(() => 12));
    }

    [Test]
    public void NumberOfThreadsTest()
    {
        Assert.AreEqual(1, threadPool.TotolNumberOfThreads);
    }
    
    [Test]
    public void ContinueWithTest()
    {
        var task = threadPool.Add(() => 111);
        var task2 = task.ContinueWith((x) => 2*x);
        Assert.AreEqual(222, task2.Result);
    }
    /*
    [Test]
    public void AfterShutDownTask()
    {
        var task1 = threadPool.Add(() => 1);
        var task2 = threadPool.Add(() => 2);
        var task3 = threadPool.Add(() => 3);
        threadPool.ShutDown();
        Assert.Throws<InvalidOperationException>(() => threadPool.Add(() => 12));
        Assert.AreEqual(1, task1.Result);
        Assert.AreEqual(2, task2.Result);
        Assert.AreEqual(3, task3.Result);
    }
    */
    /*
    [Test]
    public void ContinueWithAfterCancelTest()
    {
        var threadPool = new MyThreadPool.MyThreadPool(2);
        var task = threadPool.Add(() => 111);
        threadPool.ShutDown();
        Assert.Throws<InvalidOperationException>(() => task.ContinueWith((x) => 222));
        Assert.AreEqual(111, task.Result);
    }
    */

    [TearDown]
    public void ThreadPoolShutDown()
    {
        threadPool.ShutDown();
    }
}