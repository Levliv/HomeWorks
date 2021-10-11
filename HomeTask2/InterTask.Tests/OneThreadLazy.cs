using NUnit.Framework;
using System;
using System.Threading;

namespace LazyFactoryNamespace.Tests
{
    public class Tests
    {
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateOneThreadLazy<object>(null));
        }

        [TestCase(10, 10)]
        [TestCase(20, 20)]
        public int OneThreadLazyTest(int funcValue)
        {
            Func<int> func =() => funcValue;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            return lazy.Get();
        }
    }
}