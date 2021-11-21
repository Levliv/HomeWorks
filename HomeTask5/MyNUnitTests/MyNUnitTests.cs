using NUnit.Framework;
using MyNUnit;
using System.Linq;
using System;

namespace MyNUnitTests
{
    /// <summary>
    /// Class with tests for MyNUnit
    /// </summary>
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

        /// <summary>
        /// Testing Expected arg in MyTest attribute
        /// </summary>
        [Test]
        public void TestingExpectedInMyTest()
        {
            var resultBlock = new bool[] { true, true, true, true, true };
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            TestRunner.Start(baseLoopBack + "Test4\\");
            var MethodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "MyTest4" select i;
            foreach(var methodWithExpected in MethodsWithExpected)
            {
                Assert.AreEqual(10, methodWithExpected.Expected);
            }
        }

        /// <summary>
        /// Testing Ignore arg in MyTest attribute
        /// </summary>
        [Test]
        public void TestingIgnoreInMyTest()
        {
            var resultBlock = new bool[] { true, true, true, true, true };
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            TestRunner.Start(baseLoopBack + "Test5\\");
            var MethodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "Test5Method" select i;
            foreach (var methodWithExpected in MethodsWithExpected)
            {
                Assert.True(methodWithExpected.IsIgnored);
                Assert.AreEqual("TestIgnoreMessage", methodWithExpected.Ignore_message);
            }
        }

        /// <summary>
        /// Testing Expected as Error arg in MyTest attribute
        /// </summary>
        [Test]
        public void TestingExpectedErrorInMyTest()
        {
            var resultBlock = new bool[] { true, true, true, true, true };
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            TestRunner.Start(baseLoopBack + "Test6\\");
            var MethodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "Test6Method" select i;
            foreach (var methodWithExpected in MethodsWithExpected)
            {
                Assert.AreEqual(new ArgumentException(), methodWithExpected.Expected);         
            }
        }
    }
}