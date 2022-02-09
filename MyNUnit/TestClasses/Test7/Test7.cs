namespace Test7;

using MyAttributes;

public class Test7
{
    public static bool[] checker { get; set; } = { false, false, false };

    [BeforeClass]
    public void BeforeClassMethod()
    {
        checker[0] = true;
    }

    [MyTest]
    public void MyTestMethod()
    {
        checker[1] = true;
    }

    [AfterClass]
    public void AfterClassMethod()
    {
        checker[2] = true;
    }
}