namespace Test3;

using MyAttributes;

public class Test3
{
    /// <summary>
    /// String to be sure in order of invokation
    /// </summary>
    public static bool[] checker { get; set; } = { false, false, false, false, false };

    /// <summary>
    /// Adds one symbol to the test string
    /// </summary>
    [MyTest]
    public void MainMethod()
    {
        checker[0] = true;
    }

    /// <summary>
    /// Testing Before mark
    /// </summary>
    [Before]
    public void BeforeMethod()
    {
        checker[1] = true;
    }

    /// <summary>
    /// Second Before Method to chack that we run all of them if there are some
    /// </summary>
    [Before]
    public void BeforeMethod2()
    {
        checker[2] = true;
    }

    /// <summary>
    /// After mark tested
    /// </summary>
    [After]
    public void AfterMethod()
    {
        checker[3] = true;
    }

    /// <summary>
    /// Second After Method to chack that we run all of them if there are some
    /// </summary>
    [After]
    public void AfterMethod2()
    {
        checker[4] = true;
    }
}