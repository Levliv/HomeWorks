using NUnit.Framework;
using MyNUnit;
using System.Collections.Concurrent;

namespace MyNUnitTests
{
    public class Tests
    {
        /// <summary>
        /// Chechking that when only method have attributes everything works fine
        /// </summary>
        [Test]
        public void OneMethoWithMyTestAttribute()
        {
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            Test1.Test1.WasInvoked = false;
            TestRunner.Start(baseLoopBack + "Test1\\");
            Assert.AreEqual(true, Test1.Test1.WasInvoked);
        }

        /// <summary>
        /// Checking, when there is one method with MyTest and one with before
        /// </summary>
        [Test]
        public void OneMethoWithBeforeAttribute()
        {
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            TestRunner.Start(baseLoopBack + "Test2\\");
            Assert.AreEqual("ab", Test2.Test2.Checker);
        }

        /// <summary>
        /// Many before and after attrubutes
        /// </summary>
        [Test]
        public void MultiBeforeAndAfterAndMyTestAttribute()
        {
            var resultBlock = new bool[]{ true, true, true, true, true};
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            TestRunner.Start(baseLoopBack + "Test3\\");
            Assert.AreEqual(resultBlock, Test3.Test3.array);
        }
    }
}