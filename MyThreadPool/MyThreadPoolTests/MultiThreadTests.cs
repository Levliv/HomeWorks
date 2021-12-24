using NUnit.Framework;
using System;
using System.Threading;

namespace MyThreadPoolTests
{
    public class MultiThreadTests
    {
        /// <summary>
        /// Testing negtive number of threads
        /// </summary>
        [Test]
        public void NegativeThreadsTest()
        {
            Assert.Throws<ArgumentOutOfRangeException> (() => new MyThreadPool.MyThreadPool(-3));
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
    }
}