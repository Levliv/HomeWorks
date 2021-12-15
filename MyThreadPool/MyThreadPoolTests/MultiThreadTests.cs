using NUnit.Framework;
using System;
using System.Threading;

namespace MyThreadPoolTests
{
    public class MultiThreadTests
    {
        /// <summary>
        /// Testing negtive number of threads
        /// </summary>
        [Test]
        public void NegativeThreadsTest()
        {
            Assert.Throws<ArgumentOutOfRangeException> (() => new MyThreadPool.MyThreadPool(-3));
        }
    }
}