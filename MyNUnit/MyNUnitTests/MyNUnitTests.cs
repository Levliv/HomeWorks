namespace MyNUnitTests;

using NUnit.Framework;
using MyNUnit;
using System.Linq;
using System;
using Test1;
using Test2;
using Test3;

public class MyNUnitTests
{
    private readonly string baseLoopBackDirecroty = "..\\..\\..\\..\\TestClasses\\";
   
    [OneTimeSetUp]
    public void DllStarter()
    {
        TestRunner.Start(baseLoopBackDirecroty);
    }
    
    /// <summary>
    /// Checking that when only method have attributes everything works fine
    /// </summary>
    [Test]
    public void OneMethodWithMyTestAttribute()
    {
        TestRunner.PrintTestResults();
        Assert.IsTrue(Test1.WasInvoked);
        TestRunner.PrintTestResults();
    }
    
    /// <summary>
    /// Checking, when there is one method with MyTest and one with before
    /// </summary>
    [Test]
    public void OneMethodWithBeforeAttribute()
    {
        Assert.AreEqual("ab", Test2.Checker);
    }

    /// <summary>
    /// Multi before and after attrubutes
    /// </summary>
    [Test]
    public void MultiBeforeAndAfterAndMyTestAttribute()
    {
        var resultBlock = new bool[] { true, true, true, true, true };
        Assert.AreEqual(resultBlock, Test3.array);
    }

    /// <summary>
    /// Testing Expected arg in MyTest attribute
    /// </summary>
    [Test]
    public void TestingExpectedInMyTest()
    {
        var methodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "Test4Method" select i;
        var methodWithExpected = methodsWithExpected.Last();
        TestRunner.PrintTestResults();
        Assert.AreEqual(typeof(ArgumentOutOfRangeException), methodWithExpected.Expected);
    }

    /// <summary>
    /// Testing Ignore arg in MyTest attribute
    /// </summary>
    [Test]
    public void TestingIgnoreInMyTest()
    {
        var MethodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "Test5Method" select i;
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
        var MethodsWithExpected = from i in TestRunner.MyTests where i.MethodInformation.Name == "Test6Method" select i;
        var MethodWithExpected = MethodsWithExpected.Last();
        Assert.AreEqual(typeof(ArgumentException), MethodWithExpected.Expected);
    }
}