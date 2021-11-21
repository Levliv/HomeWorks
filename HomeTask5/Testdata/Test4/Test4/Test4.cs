using System;
using MyNUnit;

namespace Test4
{
    public class Test4
    {
        /// <summary>
        /// Testing expected with MyTest mark
        /// </summary>
        /// <returns></returns>
        [MyTest(Expected = 10)]
        public int Test4Method()
        {
            return 10;
        }
    }
}
