using MyNUnit;

namespace Test1
{
    /// <summary>
    /// The first class for test(testing simply method invokation with MyNUnit test mark)
    /// </summary>
    public class Test1
    {
        public static bool WasInvoked { get; set; }
        [MyTest]
        public void FirstMethod()
        {
            WasInvoked = true;
        }
        public void SecondMethod()
        {
            WasInvoked = false;
        }
    }
}
