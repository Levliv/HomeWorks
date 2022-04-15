namespace Test6;

using MyAttributes;

public class Test6
{
    [MyTest(Expected = typeof(ArgumentException))]
    public void Test6Method()
    {
        throw new ArgumentException();
    }
}