namespace Test4;

using MyAttributes;

/// <summary>
/// Simple expected test class.
/// </summary>
public class Test4
{
    /// <summary>
    /// Testing expected with MyTest mark.
    /// </summary>
    [MyTest(Expected = typeof(ArgumentOutOfRangeException))]
    public int Test4Method()
    {
        throw new ArgumentOutOfRangeException("Test Exception");
    }
}
