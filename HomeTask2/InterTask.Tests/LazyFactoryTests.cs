using NUnit.Framework;
using System;
using System.Threading;

namespace InterTask.Tests
{
    public class Tests
    {
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateOneThreadLazy<object>(null));
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateMultiThreadLazy<object>(null));
        }

        [Test]
        public void OneThreadLazyGet()
        {
            var random = new Random();
            Func<int> func = () => random.Next();
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            Assert.AreEqual(lazy.Get(), lazy.Get());
        }

        [Test]
        public void OneThreadLazyTest()
        {
            Func<int> func = () => 5;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            for (int i = 0; i < 10; ++i)
            {
                lazy.Get();
            }
        }

        [Test]
        public void MultiThreadLazy()
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