using System;
using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Template for data restored after test invocation
    /// </summary>
    public class TestStrcuct
    {
        /// <summary>
        /// Containing the information about the method Tested
        /// </summary>
        public MethodInfo MethodInformation { get; }

        /// <summary>
        /// Containing the information about the error message, if case it occurs
        /// </summary>
        public String ErrorMessage { get; }

        /// <summary>
        /// Containing true in case test passed
        /// </summary>
        public bool IsPassed { get; }

        /// <summary>
        /// Containing true in case test failed
        /// </summary>
        public bool IsFailed { get; }

        /// <summary>
        /// Containing true in case test is ignored
        /// </summary>
        public bool IsIgnored { get; }

        /// <summary>
        /// Containing the inforaming about the time test consumed
        /// </summary>
        public long TimeConsumed { get; }

        /// <summary>
        /// Containing the expected result of the test in case it is specified
        /// </summary>
        public object Expected { get; }

        /// <summary>
        /// Containing the actual(returned from the method) value got by test invocation  
        /// </summary>
        public object Got { get; }

        /// <summary>
        /// Containing message in case test is suppposed to be ignored
        /// </summary>
        public string IgnoreMessage { get; }

        /// <summary>
        /// Constructor to store the information about tests
        /// </summary>
        public TestStrcuct(MethodInfo methodInfo, bool isPassed = false, bool isFailed = false,
            bool isIgnored = false, long timeConsumed = 0, string errorMessage = null, object expected = null, object got = null,  string ignoreMessage = null)
        {
            MethodInformation = methodInfo;
            IsPassed = isPassed;
            IsFailed = isFailed;
            IsIgnored = isIgnored;
            ErrorMessage = errorMessage;
            Expected = expected;
            Got = got;
            IgnoreMessage = ignoreMessage;
            TimeConsumed = timeConsumed;
        }
    }
}
