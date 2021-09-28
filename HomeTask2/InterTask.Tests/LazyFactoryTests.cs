using NUnit.Framework;
using System;
using InterTask;
using System.Threading;

namespace InterTask.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DelegateIsNullOneThreadFactory()
        {
            Assert.Throws<Exception>(() => LazyFactory.CreateOneThreadLazy<object>(null));
        }

        [Test]
        public void DelegateIsNullMultiThreadFactory()
        {
            Assert.Throws<Exception>(() => LazyFactory.CreateMultiThreadLazy<object>(null));
        }

        [Test]
        public void OneThreadLazyGet()
        {
            var random = new Random();
            Func<int> func = () => random.Next();
            var lazy = LazyFactory.CreateOneThreadLazy<int>(func);
            Assert.AreEqual(lazy.Get(), lazy.Get());
        }

        [Test]
        public void MultiThreadLazy()
        {
            Func<int> func = () => 5;
            var lazy = LazyFactory.CreateMultiThreadLazy<int>(func);
            var numberOfThreads = Environment.ProcessorCount * 10;
            var threads = new Thread[numberOfThreads];
            var numbers = new int[numberOfThreads];
            int varNumber = 0;
            for (int i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(
                    () => Interlocked.Add(ref varNumber, lazy.Get())
                    );
                threads[i].Start();
            }
            
            for (int i = 0; i < numberOfThreads; ++i)
            {
                threads[i].Join();
            }
            //Console.WriteLine(numberOfThreads);
            for (int i = 0; i < numberOfThreads; ++i)
            {
                Console.WriteLine($"number = {varNumber}");
                Assert.AreEqual(varNumber, numberOfThreads*5);
            }
        }
    }
}