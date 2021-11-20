using System;
using System.Reflection;

namespace MyNUnit
{
    public class TestStrcuct
    {
        public MethodInfo MethodInformation { get; }
        public Type TypeInformation { get; }
        public String ErrorMessage { get; }
        public bool IsPassed { get; }
        public bool IsFailed { get; }
        public bool IsIgnored { get; }
        public decimal TimeConsumed { get; }
        public object Expected { get; }
        public string Ignore_message { get; }
        public TestStrcuct(MethodInfo methodInfo, bool isPassed = false, bool isFailed = false, 
            bool isIgnored = false, string errorMessage = null, object expected = null, string ignore_message = null)
        {
            MethodInformation = methodInfo;
            IsPassed = isPassed;
            IsFailed = isFailed;
            IsIgnored = isIgnored;
            ErrorMessage = errorMessage;
            Expected = expected;
            Ignore_message = ignore_message;
        }
    }
}
