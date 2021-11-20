using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace MyNUnit
{
    public static class TestRunner
    {
        public static BlockingCollection<TestStrcuct> MyTests { get; private set; }

        public static void PrintTestResults()
        {
            Console.WriteLine("Results");
            foreach (var testResult in MyTests)
            {
                Console.WriteLine($"Method : {testResult.MethodInformation}; {(testResult.IsPassed ? "Passed in " + testResult.TimeConsumed:"")}" +
                    $"{(testResult.IsFailed ? $"Failed + Expected: {testResult.Expected}, Got: {testResult.Got}":"")}" +
                    $"{((testResult.IsIgnored)? "Ignored, message = " + testResult.Ignore_message:"")}");
            }
        }
        public static void Start(string path)
        {
            MyTests = new BlockingCollection<TestStrcuct>();
            var dllFiles = Directory.GetFiles(path,"*.dll", SearchOption.AllDirectories);
            var dllFilesNotRepeated = new HashSet<string>();
            var downloadeddlls = new HashSet<string>();
            foreach (var dll in dllFiles)
            {
                if (!downloadeddlls.Contains(dll.Split("\\")[^1]))
                {
                    downloadeddlls.Add(dll.Split("\\")[^1]);
                    dllFilesNotRepeated.Add(dll);
                }

            }
            foreach(var dll in dllFilesNotRepeated)
            {
                var assembly = Assembly.LoadFrom(dll);
                var types = assembly.GetTypes();
                Parallel.ForEach(types, TestStarter);
            }
        }
        public static void TestStarter(Type type)
        {
            MethodsInvoker<BeforeClassAttribute>(type);
            MethodsInvoker<MyTestAttribute>(type);
            MethodsInvoker<AfterClassAttribute>(type);
        }
        public static void MethodsInvoker<AttributeType>(Type type, object obj = null)
        {
            Action<MethodInfo> test;
            var methodsWithAttribute = type.GetMethods().Where(x => Attribute.IsDefined(x, typeof(AttributeType)));
            if (typeof(AttributeType) == typeof(MyTestAttribute))
            {
                test = MethodsWithMyTestInvoker;
            }
            else
            if (typeof(AttributeType) == typeof(BeforeAttribute) || typeof(AttributeType) == typeof(AfterAttribute) ||
                typeof(AttributeType) == typeof(BeforeClassAttribute) || typeof(AttributeType) == typeof(AfterClassAttribute))
            {
                test = MethodsWithBeforeAndAfterClassAttribute;
            }
            else
            {
                throw new Exception("Wrong attribute type");
            }
            Parallel.ForEach(methodsWithAttribute, test);
        }
        static void MethodsWithMyTestInvoker(MethodInfo methodInfo)
        {
            MyTestAttribute attribute = (MyTestAttribute)methodInfo.GetCustomAttribute(typeof(MyTestAttribute), true);
            if (attribute.Ignore != null)
            {
                MyTests.Add(new TestStrcuct(methodInfo, isIgnored: true, ignore_message: attribute.Ignore));
            }
            else
            {
                if (attribute.Expected == null)
                {
                    object t = null;
                    bool found = false;
                    foreach (ConstructorInfo ctor in methodInfo.DeclaringType.GetConstructors())
                    {
                        ParameterInfo[] parameters = ctor.GetParameters();
                        if (parameters.Length == 0)
                        {
                            t = ctor.Invoke(null);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw new ArgumentException("Constructor with no paramets was not found");
                    }
                    MethodsInvoker<BeforeAttribute>(methodInfo.DeclaringType);
                    var watch = Stopwatch.StartNew();
                    object result = methodInfo.Invoke(t, null);
                    watch.Stop();
                    MyTests.Add(new TestStrcuct(methodInfo, isPassed: true, timeConsumed: watch.ElapsedMilliseconds));
                    MethodsInvoker<AfterAttribute>(methodInfo.DeclaringType);
                }
                else
                {
                    try
                    {
                        object t = null;
                        bool found = false;
                        foreach (ConstructorInfo ctor in methodInfo.DeclaringType.GetConstructors())
                        {
                            ParameterInfo[] parameters = ctor.GetParameters();
                            if (parameters.Length == 0)
                            {
                                t = ctor.Invoke(null);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            throw new ArgumentException("Constructor with no paramets was not found");
                        }
                        MethodsInvoker<BeforeAttribute>(methodInfo.DeclaringType);
                        var watch = Stopwatch.StartNew();
                        object result = methodInfo.Invoke(t, null);
                        watch.Stop();
                        MethodsInvoker<AfterAttribute>(methodInfo.DeclaringType);
                        if (attribute.Expected.Equals(result))
                        {
                            MyTests.Add(new TestStrcuct(methodInfo, isPassed: true, timeConsumed: watch.ElapsedMilliseconds)); ;
                        }
                        else
                        {
                            MyTests.Add(new TestStrcuct(methodInfo, isFailed: true));
                        }
                    }
                    catch (Exception exception)
                    {
                        if (attribute.Expected.Equals(exception.InnerException.GetType()))
                        {
                            MyTests.Add(new TestStrcuct(methodInfo, isPassed: true));
                        }
                        else
                        {
                            MyTests.Add(new TestStrcuct(methodInfo, isFailed: true));
                        }
                    }
                }
            }
        }

        static void MethodsWithBeforeAndAfterClassAttribute(MethodInfo methodInfo)
        {
            if (methodInfo.IsStatic && ((methodInfo.GetCustomAttribute(typeof(BeforeClassAttribute)) != null) || (methodInfo.GetCustomAttribute(typeof(AfterClassAttribute)) != null)))
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                throw new InvalidOperationException("Method to call must me static");
            }
        }
    }
}
