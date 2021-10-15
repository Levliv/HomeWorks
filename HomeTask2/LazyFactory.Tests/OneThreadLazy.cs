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

        [Test]
        public void OneThreadLazyGetCalledOnlyOnce()
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

        private static readonly object[] TestCases =
        {
            new Func<int>[] {()=> 32 },
            new Func<int>[] {()=> 2 },
            new Func<int>[] {()=> 4 },
        };

        [Test, TestCaseSource(nameof(TestCases))]
        public void Tests2(Func<int> func)
        {
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            Assert.AreEqual(lazy.Get(), func());
        }
    }
}