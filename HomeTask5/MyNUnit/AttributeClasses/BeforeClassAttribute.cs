namespace MyNUnit;

using System;

/// <summary>
/// Label for after-class tests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{
}