using System;
using System.Reflection;

namespace MyNUnit
{
    public class TestStrcuct
    {
        public MethodInfo MethodInformation { get; }
        public String ErrorMessage { get; }
        public bool IsPassed { get; }
        public bool IsFailed { get; }
        public bool IsIgnored { get; }
        public long TimeConsumed { get; }
        public object Expected { get; }
        public object Got { get; }
        public string Ignore_message { get; }
        public TestStrcuct(MethodInfo methodInfo, bool isPassed = false, bool isFailed = false,
            bool isIgnored = false, long timeConsumed = 0, string errorMessage = null, object expected = null, object got = null,  string ignore_message = null)
        {
            MethodInformation = methodInfo;
            IsPassed = isPassed;
            IsFailed = isFailed;
            IsIgnored = isIgnored;
            ErrorMessage = errorMessage;
            Expected = expected;
            Got = got;
            Ignore_message = ignore_message;
            TimeConsumed = timeConsumed;
        }
    }
}
