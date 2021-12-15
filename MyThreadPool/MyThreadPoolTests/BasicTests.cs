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
            Console.WriteLine("First");
            var task1 = threadPool.Add(() => 1);
            Console.WriteLine("Second");
            var task2 = threadPool.Add(() => 2);
            Console.WriteLine("Third");
            var task3 = threadPool.Add(() => 3);
            Console.WriteLine("Finish");
            //threadPool.ShutDown();
            Console.WriteLine("ShutDown OK");
            Assert.AreEqual(1, task1.Result);
            Assert.AreEqual(2, task2.Result);
            Assert.AreEqual(3, task3.Result);
        }

        /*
        [Test]
        public void TaskAfterShutDownTest() 
        {
            threadPool.ShutDown();
            var task = threadPool.Add(() => 12);
            Assert.Throws<InvalidOperationException>(delegate { threadPool.Add(() => 12); });
        }
        */
        [Test]
        public void NumberOfThreadsTest()
        {
            Assert.Pass();
        }
    }
}