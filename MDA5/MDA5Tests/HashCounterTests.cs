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
            Console.WriteLine(HashCounter.ComputeHashSingleThread("..\\..\\..\\..\\Tests"));
            Assert.Pass();
        }
    }
}