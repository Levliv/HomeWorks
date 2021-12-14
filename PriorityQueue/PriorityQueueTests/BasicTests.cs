using NUnit.Framework;
using System.Threading;
using System;

namespace PriorityQueueTests;
public class Tests
{

    [Test]
    public void SingleThreadCorrectness()
    {
        var priorityQueue = new PriorityQueue.PriorityQueue();
        for (int i = 0; i < 6; ++i)
        {
            priorityQueue.Enqueue(i, i);
        }
        for (int i = 5; i >= 0; --i)
        {
            Assert.AreEqual(i, priorityQueue.Dequeue());
        }
    }

    [Test]
    public void MultiThreadTest()
    {
        var priorityQueue = new PriorityQueue.PriorityQueue();
        var thread1 = new Thread(() =>
        {
            priorityQueue.Enqueue(12, 1);
        });
        var thread2 = new Thread(() =>
        {
            priorityQueue.Enqueue(22, 5);
        });
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
        Assert.AreEqual(22, priorityQueue.Dequeue());
        Assert.AreEqual(12, priorityQueue.Dequeue());
    }
    [Test]
    public void TestSize()
    {
        var priorityQueue = new PriorityQueue.PriorityQueue();
        for (int i = 0; i < 200; ++i)
        {
            priorityQueue.Enqueue(i, i);
        }
        Assert.AreEqual(200, priorityQueue.Size());
    }
}