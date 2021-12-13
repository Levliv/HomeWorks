using NUnit.Framework;
using MDA5;
using System;

namespace MDA5Tests
{
    public class Tests
    {

        [Test]
        public void HashDoesNotChangeTest()
        {
            Assert.AreEqual("88-CB-42-90-00-50-CA-9D-45-39-2B-46-F8-DF-26-40", BitConverter.ToString(HashCounter.ComputeHashSingleThread("..\\..\\..\\..\\Tests")));
        }
        [Test]
        public void HashMultiThreadTest()
        {
            Assert.AreEqual("88-CB-42-90-00-50-CA-9D-45-39-2B-46-F8-DF-26-40", BitConverter.ToString(HashCounter.ComputeHashMultiThread("..\\..\\..\\..\\Tests")));
        }
    }
}