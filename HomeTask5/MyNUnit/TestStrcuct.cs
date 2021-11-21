using System;
using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Template for data restored after test invokation
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
        /// Containg the actual(returned from the method) value got by test invocation 
        /// </summary>
        public object Got { get; }

        /// <summary>
        /// Containig message is case test in suppposed to be ignored
        /// </summary>
        public string Ignore_message { get; }

        /// <summary>
        /// Constructor to restore the information about tests
        /// </summary>
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
