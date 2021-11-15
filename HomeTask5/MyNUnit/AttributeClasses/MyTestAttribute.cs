using System;

namespace MyNUnit
{
    /// <summary>
    /// A meta data for Tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MyTestAttribute : Attribute
    {
        public object Expected = null;
        public string Ignore = null;
    }
}