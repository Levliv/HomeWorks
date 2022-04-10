namespace MyNUnit;

using System;
using System.Reflection;

/// <summary>
/// Template for data restored after test invocation.
/// </summary>
public class TestStrcuct
{
    /// <summary>
    /// Gets the information about the method Tested.
    /// </summary>
    public MethodInfo MethodInformation { get; }

    /// <summary>
    /// Gets the information about the error message, if case it occurs.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets a value indicating whether test passed.
    /// </summary>
    public bool IsPassed { get; }

    /// <summary>
    /// Gets a value indicating whether test failed.
    /// </summary>
    public bool IsFailed { get; }

    /// <summary>
    /// Gets a value indicating whether test is ignored.
    /// </summary>
    public bool IsIgnored { get; }

    /// <summary>
    /// Gets the inforaming about the time test consumed.
    /// </summary>
    public long TimeConsumed { get; }

    /// <summary>
    /// Gets the expected result of the test in case it is specified.
    /// </summary>
    public object Expected { get; }

    /// <summary>
    /// Gets the actual (returned from the method) value got by test invocation.
    /// </summary>
    public object Got { get; }

    /// <summary>
    /// Gets message in case test is suppposed to be ignored.
    /// </summary>
    public string IgnoreMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestStrcuct"/> class.
    /// Constructor to store the information about tests.
    /// </summary>
    public TestStrcuct(MethodInfo methodInfo, bool isPassed = false, bool isFailed = false,
        bool isIgnored = false, long timeConsumed = 0, string? errorMessage = null, object? expected = null, object? got = null, string? ignoreMessage = null)
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