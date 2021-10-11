using NUnit.Framework;
using System;
using System.Threading;

namespace LazyFactoryNamespace.Tests
{
    class MultiThreadLazy
    {
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateMultiThreadLazy<object>(null));
        }

        [TestCase(10, ExpectedResult=10)]
        [TestCase(20, ExpectedResult=20)]
        public int MultiThreadLazyTest(int funcValue)
        {
            Func<int> func = () => funcValue;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            return lazy.Get();
        }

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
            Assert.AreEqual(counter, 1);
        }
    }
}
