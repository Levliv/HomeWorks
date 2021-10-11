using NUnit.Framework;
using System;

namespace LazyFactoryNamespace.Tests
{
    public class Tests
    {
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateOneThreadLazy<object>(null));
        }

        [TestCase(10, ExpectedResult=10)]
        [TestCase(20, ExpectedResult=20)]
        public int OneThreadLazyTest(int funcValue)
        {
            Func<int> func =() => funcValue;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            return lazy.Get();
        }
    }
}