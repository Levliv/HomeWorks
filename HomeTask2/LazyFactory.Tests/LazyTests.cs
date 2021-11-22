using NUnit.Framework;
using System;
using System.Threading;

namespace LazyFactoryNamespace.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateOneThreadLazy<object>(null));
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateMultiThreadLazy<object>(null));
        }

        private static readonly object[] TestCases =
        {
            new Func<object>[] { ()=> 32},
            new Func<object>[] { ()=> "abc"},
            new Func<object>[] { ()=> 'c'},
        };

        [Test, TestCaseSource(nameof(TestCases))]
        public void TestsOneAndMultiThread(Func<object> func)
        {
            Assert.AreEqual(LazyFactory.CreateMultiThreadLazy(func).Get(), func());
            Assert.AreEqual(LazyFactory.CreateOneThreadLazy(func).Get(), func());
        }

        [Test]
        public void OneThreadLazyGetCalledOnlyOnceTest()
        {
            var counter = 0;
            Func<int> func = () => counter++;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            for (int i = 0; i < 2; ++i)
            {
                lazy.Get();
            }
            Assert.AreEqual(counter, 1);
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