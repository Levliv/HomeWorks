using NUnit.Framework;
using System;
using System.Threading;

namespace MyLazy
{
    [TestFixture]
    class MultiThreadLazyTest
    {
        /// <summary>
        /// Test for null supplier
        /// </summary>
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateMultiThreadLazy<object>(null));
        }


        /// <summary>
        /// Testing Race condition for Multi thread Lazy version
        /// </summary>
        [Test]
        public void RaceMultiThreadLazyTest()
        {
            int counter = 0;
            Func<int> func = () => 5;
            Func<int> func2 = () => Interlocked.Increment(ref counter);
            func += func2;
            var lazy = LazyFactory.CreateMultiThreadLazy(func);
            var numberOfThreads = Environment.ProcessorCount * 10;
            var threads = new Thread[numberOfThreads];
            var numbers = new int[numberOfThreads];
            for (int i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(
                    () => lazy.Get()
                    );
                threads[i].Start();
            }
            for (int i = 0; i < numberOfThreads; ++i)
            {
                threads[i].Join();
            }
            Assert.AreEqual(1, counter);
        }
    }
}
