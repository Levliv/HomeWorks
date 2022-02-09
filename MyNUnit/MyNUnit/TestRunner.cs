namespace MyNUnit;

using MyAttributes;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Class for MyNUnit Framework.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Concurent collection to strore data about the tests.
    /// </summary>
    public static BlockingCollection<TestStrcuct> MyTests { get; private set; }

    /// <summary>
    /// Printing the results of testing.
    /// </summary>
    public static void PrintTestResults()
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
    /// Loadign the dll and starting the tests, Cheks for not repeated dlls.
    /// </summary>
    /// <param name="path">Path to folder, loading all the dlls in all the directorieds beneath it as well.</param>
    public static void Start(string path)
    {
        MyTests = new BlockingCollection<TestStrcuct>();
        var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        var dllFilesNotRepeated = new HashSet<string>();
        var downloadedDlls = new HashSet<string>();
        foreach (var dll in dllFiles)
        {
            if (!downloadedDlls.Contains(dll.Split("\\")[^1]))
            {
                downloadedDlls.Add(dll.Split("\\")[^1]);
                dllFilesNotRepeated.Add(dll);
            }
        }

        Parallel.ForEach(dllFilesNotRepeated, dll =>
        {
            var assembly = Assembly.LoadFrom(dll);
            var types = assembly.GetTypes();
            Parallel.ForEach(types, TestStarter);
        });
    }

    /// <summary>
    /// Starting the all the tests with BeforeClass - Before - MyTest - After - AfterClass atrributes.
    /// </summary>
    /// <param name="type">loaded assembly.</param>
    public static void TestStarter(Type type)
    {
        MethodsInvoker<BeforeClassAttribute>(type);
        MethodsInvoker<MyTestAttribute>(type);
        MethodsInvoker<AfterClassAttribute>(type);
    }

    /// <summary>
    /// Invokes the methods with attribues, calling methods corresponding to the attribute type.
    /// </summary>
    /// <typeparam name="AttributeType">BeforeClass - Before - MyTest - After - AfterClass atrributes.</typeparam>
    public static void MethodsInvoker<AttributeType>(Type type, object obj = null)
    {
        Action<MethodInfo> test;
        var methodsWithAttribute = type.GetMethods().Where(x => Attribute.IsDefined(x, typeof(AttributeType)));
        if (typeof(AttributeType) == typeof(MyTestAttribute))
        {
            test = x => MethodsWithMyTestInvoker(x, obj);
        }
        else
        if (typeof(AttributeType) == typeof(BeforeClassAttribute) || typeof(AttributeType) == typeof(AfterClassAttribute))
        {
            test = x => MethodsWithBeforeAndAfterClassAttribute(x, obj);
        }
        else
        if (typeof(AttributeType) == typeof(BeforeAttribute) || typeof(AttributeType) == typeof(AfterAttribute))
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
    public static void MethodsWithMyTestInvoker(MethodInfo methodInfo, object obj)
    {
        obj = Activator.CreateInstance(methodInfo.DeclaringType);
        MyTestAttribute attribute = (MyTestAttribute)methodInfo.GetCustomAttribute(typeof(MyTestAttribute), true);
        if (attribute.Ignore != null)
        {
            MyTests.Add(new TestStrcuct(methodInfo, isIgnored: true, ignoreMessage: attribute.Ignore));
            return;
        }

        MethodsInvoker<BeforeAttribute>(methodInfo.DeclaringType, obj);
        var watch = Stopwatch.StartNew();
        try
        {
            methodInfo.Invoke(obj, null);
        }
        catch (Exception exception)
        {
            watch.Stop();
            if (attribute.Expected.Equals(exception.InnerException.GetType()))
            {
                MyTests.Add(new TestStrcuct(methodInfo, expected: attribute.Expected, isPassed: true));
            }
            else
            {
                MyTests.Add(new TestStrcuct(methodInfo, expected: attribute.Expected, isFailed: true));
            }
        }
        MethodsInvoker<AfterAttribute>(methodInfo.DeclaringType, obj);
    }

    /// <summary>
    /// Invoking methods with Before and After Attribute.
    /// </summary>
    public static void MethodsWithAfterAndBeforeAttribute(MethodInfo methodInfo, object obj = null)
    {
        methodInfo.Invoke(obj, null);
    }

    /// <summary>
    /// Invoking methods with BeforeClass and AfterClass Attributes, without creating an instance, requires methods to be static.
    /// </summary>
    public static void MethodsWithBeforeAndAfterClassAttribute(MethodInfo methodInfo, object obj)
    {
        Console.WriteLine("Fuck it ");
        if (!methodInfo.IsStatic && ((methodInfo.GetCustomAttribute(typeof(BeforeClassAttribute)) != null) || (methodInfo.GetCustomAttribute(typeof(AfterClassAttribute)) != null)))
        {
            throw new InvalidOperationException("Method to call must me static");
        }

        methodInfo.Invoke(obj, null);
    }
}