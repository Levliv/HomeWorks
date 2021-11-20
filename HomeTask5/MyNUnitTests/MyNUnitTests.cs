using NUnit.Framework;
using System;
using System.IO;
using MyNUnit;

namespace MyNUnitTests
{
    public class Tests
    {
        /// <summary>
        /// Chechking that when only method have attributes everything works fine
        /// </summary>
        [Test]
        public void MethodsAttributes()
        {
            var baseLoopBack = "../../../../Testdata/";
            TestRunner.Start(baseLoopBack + "Test1/");

            Assert.Pass();
        }
    }
}