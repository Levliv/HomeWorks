namespace MyNUnit;

using System;

/// <summary>
/// Method with this label will invoke after each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : MyTestAttribute
{
}