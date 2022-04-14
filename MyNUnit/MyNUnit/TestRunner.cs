namespace MyNUnit;

using MyAttributes;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Class for MyNUnit Framework.
/// </summary>
public class TestRunner
{
    /// <summary>
    /// Concurent collection to strore data about the tests.
    /// </summary>
    public BlockingCollection<TestStruct> MyTests { private set; get; } = new();

    /// <summary>
    /// Printing the results of testing.
    /// </summary>
    public void PrintTestResults()
    {
        Console.WriteLine("Results");
        foreach (var testResult in MyTests)
        {
            Console.WriteLine($"Method : {testResult.MethodInformation}; {(testResult.IsPassed ? "Passed in " + testResult.TimeConsumed : string.Empty)}" +
                $"{(testResult.IsFailed ? $"Failed + Expected: {testResult.Expected}, Got: {testResult.Got}" : string.Empty)}" +
                $"{(testResult.IsIgnored ? "Ignored, message = " + testResult.IgnoreMessage : string.Empty)}");
        }
    }

    /// <summary>
    /// Loading the dll and starting the tests, Checks for not repeated dlls.
    /// </summary>
    /// <param name="path">Path to folder, loading all the dlls in all the directories beneath it as well.</param>
    public void Start(string path)
    {
        var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        var dllFilesNotRepeated = new HashSet<string>();
        var loadedDlls = new HashSet<string>();
        foreach (var dll in dllFiles)
        {
            if (!loadedDlls.Contains(dll.Split("\\")[^1]))
            {
                loadedDlls.Add(dll.Split("\\")[^1]);
                dllFilesNotRepeated.Add(dll);
            }
        }

        Parallel.ForEach(dllFilesNotRepeated, dll =>
        {
            var assembly = Assembly.LoadFrom(dll);
            var types = assembly.GetTypes();
            Parallel.ForEach(types, StartTests);
        });
    }

    /// <summary>
    /// Starting the all the tests with BeforeClass - Before - MyTest - After - AfterClass atrributes.
    /// </summary>
    /// <param name="type">loaded assembly.</param>
    public void StartTests(Type type)
    {
        InvokeMethods<BeforeClassAttribute>(type);
        InvokeMethods<MyTestAttribute>(type);
        InvokeMethods<AfterClassAttribute>(type);
    }

    /// <summary>
    /// Invokes the methods with attributes, calling methods corresponding to the attribute type.
    /// </summary>
    /// <typeparam name="AttributeType">BeforeClass - Before - MyTest - After - AfterClass aattributes.</typeparam>
    public void InvokeMethods<AttributeType>(Type type, object? obj = null)
    {
        Action<MethodInfo> test;
        var methodsWithAttribute = type.GetMethods().Where(x => Attribute.IsDefined(x, typeof(AttributeType)));
        if (typeof(AttributeType) == typeof(MyTestAttribute))
        {
            test = x => MethodsWithMyTestInvoker(x);
        }
        else if (typeof(AttributeType) == typeof(BeforeClassAttribute) || typeof(AttributeType) == typeof(AfterClassAttribute))
        {
            test = x => MethodsWithBeforeAndAfterClassAttribute(x, obj);
        }
        else if (typeof(AttributeType) == typeof(BeforeAttribute) || typeof(AttributeType) == typeof(AfterAttribute))
        {
            test = x => MethodsWithAfterAndBeforeAttribute(x, obj);
        }
        else
        {
            throw new Exception("Wrong attribute type");
        }

        Parallel.ForEach(methodsWithAttribute, test);
    }

    /// <summary>
    /// Invoking Methods with MyTestAttribute.
    /// </summary>
    public void MethodsWithMyTestInvoker(MethodInfo methodInfo)
    {
        ArgumentNullException.ThrowIfNull(methodInfo.DeclaringType);
        var obj = Activator.CreateInstance(methodInfo.DeclaringType);
        var customAttribute = methodInfo.GetCustomAttribute(typeof(MyTestAttribute), true);
        ArgumentNullException.ThrowIfNull(customAttribute);
        var attribute = (MyTestAttribute)customAttribute;
        ArgumentNullException.ThrowIfNull(attribute);
        if (attribute.Ignore != null)
        {
            MyTests.Add(new TestStruct(methodInfo, isIgnored: true, ignoreMessage: attribute.Ignore));
            return;
        }

        InvokeMethods<BeforeAttribute>(methodInfo.DeclaringType, obj);
        var watch = Stopwatch.StartNew();
        try
        {
            methodInfo.Invoke(obj, null);
            watch.Stop();
            if (attribute.Expected == null)
            {
                MyTests.Add(new TestStruct(methodInfo, expected: attribute.Expected, isPassed: true, timeConsumed: watch.ElapsedMilliseconds));
            }
            else
            {
                MyTests.Add(new TestStruct(methodInfo, expected: attribute.Expected, isFailed: true));
            }
        }
        catch (Exception exception)
        {
            watch.Stop();
            ArgumentNullException.ThrowIfNull(exception.InnerException);
            if (attribute.Expected == exception.InnerException.GetType())
            {
                MyTests.Add(new TestStruct(methodInfo, expected: attribute.Expected, isPassed: true, timeConsumed: watch.ElapsedMilliseconds));
            }
            else
            {
                MyTests.Add(new TestStruct(methodInfo, expected: attribute.Expected, isFailed: true));
            }
        }
        InvokeMethods<AfterAttribute>(methodInfo.DeclaringType, obj);
    }

    /// <summary>
    /// Invoking methods with Before and After Attribute.
    /// </summary>
    public void MethodsWithAfterAndBeforeAttribute(MethodInfo methodInfo, object? obj)
    {
        methodInfo.Invoke(obj, null);
    }

    /// <summary>
    /// Invoking methods with BeforeClass and AfterClass Attributes, without creating an instance, requires methods to be static.
    /// </summary>
    public void MethodsWithBeforeAndAfterClassAttribute(MethodInfo methodInfo, object? obj)
    {
        if (!methodInfo.IsStatic && ((methodInfo.GetCustomAttribute(typeof(BeforeClassAttribute)) != null) || (methodInfo.GetCustomAttribute(typeof(AfterClassAttribute)) != null)))
        {
            throw new InvalidOperationException("Method to call must be static");
        }

        methodInfo.Invoke(obj, null);
    }
}