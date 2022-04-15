namespace Test2;

using MyAttributes;

public class Test2
{
    public static bool[] checker { get; set; } = { false, false, false };

    [BeforeClass]
    public static void BeforeClassMethod()
    {
        checker[0] = true;
    }

    [MyTest]
    public void MyTestMethod()
    {
        checker[1] = true;
    }

    [AfterClass]
    public static void AfterClassMethod()
    {
        checker[2] = true;
    }
}