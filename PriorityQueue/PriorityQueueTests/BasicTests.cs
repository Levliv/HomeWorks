using NUnit.Framework;
using System.Threading;
using System;

namespace PriorityQueueTests;
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

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
    public void SizeTest()
    {
        var priorityQueue = new PriorityQueue.PriorityQueue();
        var threads = new Thread[4];
        for (int i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() =>
            {
                priorityQueue.Enqueue(i, i);
            });
            threads[i].Start();
        }
        for (int i = 0; i < threads.Length; ++i)
        {
            threads[i].Join();
        }
        for (int i = threads.Length; i > 0; --i)
        {
            Assert.AreEqual(i, priorityQueue.Dequeue());
        }
    }
}