using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MyThreadPoolTests
{
    public class BasicTests
    {
        private MyThreadPool.MyThreadPool threadPool;
        private int counter = 0;

        [SetUp]
        public void Setup()
        {
            threadPool = new MyThreadPool.MyThreadPool(1);
            counter = 0;
        }

        [Test]
        public void MultipleTasksComputationTests()
        {
            var task1 = threadPool.Add(() => 1);
            var task2 = threadPool.Add(() => 2);
            var task3 = threadPool.Add(() => 3);
            threadPool.ShotDown();
            Assert.AreEqual(1, task1);
            Assert.AreEqual(2, task2);
            Assert.AreEqual(3, task3);
        }

        [Test]
        public void TaskAfterShutDownTest() 
        {
            threadPool.ShotDown();
            var task = threadPool.Add(() => 12);
            Assert.Throws<InvalidOperationException>(delegate { threadPool.Add(() => 12); });
        }

        [Test]
        public void NumberOfThreadsTest()
        {

        }
    }
}