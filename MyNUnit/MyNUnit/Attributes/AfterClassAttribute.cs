namespace MyAttributes;

using System;

/// <summary>
/// Label for after-class tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{
}