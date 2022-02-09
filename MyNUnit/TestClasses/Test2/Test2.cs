namespace Test2;

using MyAttributes;

public class Test2
{
    /// <summary>
    /// String to be sure in order of invokation
    /// </summary>
    public static string Checker { get; private set; } = "";

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