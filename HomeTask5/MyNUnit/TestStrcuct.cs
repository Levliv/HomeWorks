using System;
using System.Reflection;

namespace MyNUnit
{
    public class TestStrcuct
    {
        public MethodInfo MethodInformation { get; }
        public Assembly AssemblyInformation { get; }
        public Type TypeInformation { get; }
        public String ErrorMessage { get; }
        public int Status { get; }
        public decimal TimeConsumed { get; }
        public object Expected { get; }
        public string Ignore { get; }
        public TestStrcuct(MethodInfo methodInfo, Assembly assembly, Type type, 
            string errorMessage = null, object expected = null, string ignore = null)
        {
            MethodInformation = methodInfo;
            AssemblyInformation = assembly;
            TypeInformation = type;
            ErrorMessage = errorMessage;
            Expected = expected;
            Ignore = ignore;
        }
    }
}
