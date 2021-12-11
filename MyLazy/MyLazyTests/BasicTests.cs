using NUnit.Framework;
using System;
using System.Collections;
using System.Threading;

namespace MyLazy
{
    [TestFixture]
    public class OneThreadLazyTests
    {
        private static int counter = 0;

        /// <summary>
        /// Resetting the counter
        /// </summary>
        [SetUp]
        public void BeforeTest()
        {
            counter = 0;
        }

        /// <summary>
        /// Test data
        /// </summary>
        public static IEnumerable TestData = new object[] { 12, "abc", 'c' };

        /// <summary>
        /// Testing Lazies
        /// </summary>
        public static IEnumerable TestLazy()
        {
            yield return LazyFactory.CreateMultiThreadLazy(() => Interlocked.Increment(ref counter));
            yield return LazyFactory.CreateOneThreadLazy(() => counter++);
        }

        /// <summary>
        /// Tesing The correctness on Lazy Computation
        /// </summary>
        [TestCaseSource(nameof(TestData))]
        public void TestForCorrectness(object d)
        {
            var lazy = LazyFactory.CreateOneThreadLazy(() => d);
            Assert.AreEqual(d, lazy.Get());
        }

        /// <summary>
        /// Test for null supplier
        /// </summary>
        [Test]
        public void DelegateIsNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateOneThreadLazy<object>(null));
        }

        /// <summary>
        /// Testing that One thread Lazy called only once(an init as well), and never called again
        /// </summary>
        [TestCaseSource(nameof(TestLazy))]
        public void LazyGetCalledOnlyOnceTest<T>(ILazy<T> lazy)
        {
            for (int i = 0; i < 10; ++i)
            {
                lazy.Get();
            }
            Assert.AreEqual(1, counter);
        }
    }
}