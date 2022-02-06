using NUnit.Framework;
using System;

namespace MyThreadPoolTests
{
    /// <summary>
    /// Tests for multithread ThreadPool using muti tasks.
    /// </summary>
    public class MultiThreadTests
    {
        [Test]
        public void NegativeThreadsTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool.MyThreadPool(-3));
        }

        [Test]
        public void OneTaskMultiThreadComputationTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(2);
            var task1 = threadPool.Add(() => 1);
            Assert.AreEqual(1, task1.Result);
        }
        
        [Test]
        public void MultiTaskMultiThreadComputationTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(2);
            var task1 = threadPool.Add(() => 1);
            var task2 = threadPool.Add(() => 2);
            var task3 = threadPool.Add(() => 3);
            var task4 = threadPool.Add(() => 4);
            Assert.AreEqual(1, task1.Result);
            Assert.AreEqual(2, task2.Result);
            Assert.AreEqual(3, task3.Result);
            Assert.AreEqual(4, task4.Result);
        }

        [Test]
        public void AfterShutDownTask()
        {
            var threadPool = new MyThreadPool.MyThreadPool(2);
            var task1 = threadPool.Add(() => 1);
            var task2 = threadPool.Add(() => 2);
            var task3 = threadPool.Add(() => 3);
            threadPool.ShutDown();
            Assert.Throws<InvalidOperationException>(() => threadPool.Add(() => 12));
            Assert.AreEqual(1, task1.Result);
            Assert.AreEqual(2, task2.Result);
            Assert.AreEqual(3, task3.Result);
        }

        [Test]
        public void CancelAfterContinueWithTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(2);
            var task = threadPool.Add(() => 111);
            task = task.ContinueWith((x) => 222);
            threadPool.ShutDown();
            Assert.AreEqual(222, task.Result);
        }

        [Test]
        public void ContinueWithAfterCancelTest()
        {
            var threadPool = new MyThreadPool.MyThreadPool(2);
            var task = threadPool.Add(() => 111);
            threadPool.ShutDown();
            Assert.Throws<InvalidOperationException>(() => task.ContinueWith((x) => 222));
            Assert.AreEqual(111, task.Result);
        }
    }
}