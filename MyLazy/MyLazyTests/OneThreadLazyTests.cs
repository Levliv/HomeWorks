using NUnit.Framework;
using System;
using System.Collections;


namespace MyLazy
{
    [TestFixture]
    public class OneThreadLazyTests
    {
        /// <summary>
        /// Test data
        /// </summary>
        public static IEnumerable TestData = new object[] { 12, "abc", 'c' };

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
        [Test]
        public void OneThreadLazyGetCalledOnlyOnceTest()
        {
            var counter = 0;
            Func<int> func = () => counter++;
            var lazy = LazyFactory.CreateOneThreadLazy(func);
            for (int i = 0; i < 10; ++i)
            {
                lazy.Get();
            }
            Assert.AreEqual(counter, 1);
        }
    }
}