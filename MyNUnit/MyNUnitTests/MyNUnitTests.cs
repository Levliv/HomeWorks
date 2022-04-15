namespace MyNUnitTests;

using NUnit.Framework;
using MyNUnit;
using System.Linq;
using System;

public class MyNUnitTests
{
    private TestRunner testRunner = new TestRunner();

    [SetUp]
    public void TestRunnerStart()
    {
        testRunner.Start("..\\..\\..\\..\\TestClasses\\");
    }
    
    /// <summary>
    /// Checking that when only one method have attributes everything works fine
    /// </summary>
    [Test]
    public void OneMethodWithMyTestAttribute()
    {
        Assert.IsTrue(Test1.Test1.WasInvoked);
    }

    /// <summary>
    /// one with MyTest and one with before attribute
    /// </summary>
    [Test]
    public void BeforeClassAndAfterClassAttribute()
    {
        var resultBlock = new bool[] { true, true, true };
        Assert.AreEqual(resultBlock, Test2.Test2.checker);
    }

    /// <summary>
    /// Multi before and after attributes
    /// </summary>
    [Test]
    public void MultiBeforeAndAfterAndMyTestAttribute()
    {
        var resultBlock = new bool[] { true, true, true, true, true };
        Assert.AreEqual(resultBlock, Test3.Test3.checker);
    }

    /// <summary>
    /// Testing Expected arg in MyTest attribute
    /// </summary>
    [Test]
    public void TestingExpectedInMyTest()
    {
        var methodsWithExpected = from i in testRunner.MyTests where i.MethodInformation.Name == "Test4Method" select i;
        var methodWithExpected = methodsWithExpected.Last();
        Assert.AreEqual(typeof(ArgumentOutOfRangeException), methodWithExpected.Expected);
    }

    /// <summary>
    /// Testing Ignore arg in MyTest attribute
    /// </summary>
    [Test]
    public void TestingIgnoreInMyTest()
    {
        var MethodsWithExpected = from i in testRunner.MyTests where i.MethodInformation.Name == "Test5Method" select i;
        var methodWithExpected = MethodsWithExpected.Last();
        Assert.True(methodWithExpected.IsIgnored);
        Assert.AreEqual("TestIgnoreMessage", methodWithExpected.IgnoreMessage);
    }

    /// <summary>
    /// Testing Expected as Error arg in MyTest attribute
    /// </summary>
    [Test]
    public void TestingExpectedErrorInMyTest()
    {
        var MethodsWithExpected = from i in testRunner.MyTests where i.MethodInformation.Name == "Test6Method" select i;
        var MethodWithExpected = MethodsWithExpected.Last();
        Assert.AreEqual(typeof(ArgumentException), MethodWithExpected.Expected);
    }
}