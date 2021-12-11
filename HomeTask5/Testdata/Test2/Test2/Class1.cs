using MyNUnit;

namespace Test2
{
    public class Test2
    {
        /// <summary>
        /// String to be sure in order of invokation
        /// </summary>
        public static string Checker { get; set; } = "";

        /// <summary>
        /// Adds one symbol to the test string
        /// </summary>
        [MyTest]
        public void MainMethod()
        {
            Checker += "b";
        }

        /// <summary>
        /// Adds one symbol to the test string
        /// </summary>
        [Before]
        public void BeforeMethod()
        {
            Checker = "a";
        }
    }
}
