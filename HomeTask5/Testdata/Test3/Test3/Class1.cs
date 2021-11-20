using System.Collections.Concurrent;
using MyNUnit;

namespace Test3
{
    public class Test3
    {
        /// <summary>
        /// String to be sure in order of invokation
        /// </summary>
        public static BlockingCollection<int> block = new ();

        /// <summary>
        /// Adds one symbol to the test string
        /// </summary>
        [MyTest]
        public void MainMethod()
        {
            block.Add(2);
        }

        /// <summary>
        /// Testing Before mark
        /// </summary>
        [Before]
        public void BeforeMethod()
        {
            block.Add(1);
        }

        /// <summary>
        /// Second Before Method to chack that we run all of them if there are some
        /// </summary>
        [Before]
        public void BeforeMethod2()
        {
            block.Add(1);
        }

        /// <summary>
        /// After mark tested
        /// </summary>
        [After]
        public void AfterMethod()
        {
            block.Add(3);
        }
        /// <summary>
        /// Second After Method to chack that we run all of them if there are some
        /// </summary>
        [After]
        public void AfterMethod2()
        {
            block.Add(3);
        }
    }
}
