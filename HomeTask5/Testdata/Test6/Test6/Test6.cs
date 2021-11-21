using System;
using MyNUnit;

namespace Test6
{
    public class Test6
    {
        [MyTest(Expected = typeof(ArgumentException))]
        public void Test6Method()
        {
            throw new ArgumentException();
        }
    }
}
