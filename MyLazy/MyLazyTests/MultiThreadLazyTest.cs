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
        /// Testing that One thread Lazy called only once(an init as well), and never called again
        /// </summary>
        [Test]
        public void MultiThreadLazyGetCalledOnlyOnceTest()
        {
            var counter = 0;
            Func<int> func = () => counter++;
            var lazy = LazyFactory.CreateMultiThreadLazy(func);
            for (int i = 0; i < 10; ++i)
            {
                lazy.Get();
            }
            Assert.AreEqual(counter, 1);
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
