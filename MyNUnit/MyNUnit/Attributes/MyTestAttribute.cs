namespace MyAttributes;

using System;

/// <summary>
/// A meta data for Tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class MyTestAttribute : Attribute
{
    /// <summary>
    /// Expected value.
    /// </summary>
    public Type? Expected { get; set; } = null;

    /// <summary>
    /// Message in case test supposed to be ignored.
    /// </summary>
    public string? Ignore { get; set; } = null;
}