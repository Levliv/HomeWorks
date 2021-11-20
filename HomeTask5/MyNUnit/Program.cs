using System;
using System.Reflection;
using System.Collections.Generic;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseLoopBack = "..\\..\\..\\..\\Testdata\\";
            Test1.Test1.WasInvoked = false;
            TestRunner.Start(baseLoopBack + "Test1\\");
            TestRunner.PrintTestResults();
        }
    }
}
