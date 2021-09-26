using NUnit.Framework;
using System;
using InterTask;

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
    }
}