using NUnit.Framework;
using System;
using System.IO;
using MyNUnit;
using Test1;

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
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            Test1.Test1.WasInvoked = false;
            TestRunner.Start(baseLoopBack + "Test1\\");
            Assert.AreEqual(true, Test1.Test1.WasInvoked);
        }
    }
}