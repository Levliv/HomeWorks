using NUnit.Framework;
using System;

namespace MyThreadPoolTests
{
    /// <summary>
    /// Testing TPL with some one thread scenario to catch some mistakes
    /// </summary>
    public class BasicTests
    {
        private MyThreadPool.MyThreadPool threadPool;

        [SetUp]
        public void Setup()
        {
            threadPool = new MyThreadPool.MyThreadPool(1);
        }

        [Test]
        public void MultipleTasksComputationTests()
        {
            Console.WriteLine("First");
            var task1 = threadPool.Add(() => 1);
            Console.WriteLine("Second");
            var task2 = threadPool.Add(() => 2);
            Console.WriteLine("Third");
            var task3 = threadPool.Add(() => 3);
            Console.WriteLine("Finish");
            Console.WriteLine("ShutDown OK");
            Assert.AreEqual(1, task1.Result);
            Assert.AreEqual(3, task3.Result);
            Assert.AreEqual(2, task2.Result);
        }

        /// <summary>
        /// Testing that if we try to add task after shut down ThreadPool won't accept it
        /// </summary>
        [Test]
        public void TaskAfterShutDownTest()
        {
            threadPool.ShutDown();
            Assert.Throws<InvalidOperationException>(delegate { threadPool.Add(() => 12); });
        }

        [Test]
        public void NumberOfThreadsTest()
        {
            Assert.AreEqual(1, threadPool.ActiveThreads);
        }

        [Test]
        public void ContinueWithTest()
        {
            var task = threadPool.Add(() => 111);
            task = task.ContinueWith((x) => 222);
            Assert.AreEqual(222, task.Result);
        }
    }
}