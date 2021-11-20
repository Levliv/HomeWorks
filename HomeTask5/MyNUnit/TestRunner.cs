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
                test = x => MethodsWithMyTestInvoker(x, obj);
            }
            else
            if (typeof(AttributeType) == typeof(BeforeClassAttribute) || typeof(AttributeType) == typeof(AfterClassAttribute))
            {

                test = x => MethodsWithBeforeAndAfterClassAttribute(x, obj);
            }
            else
            if (typeof(AttributeType) == typeof(BeforeAttribute) || typeof(AttributeType) == typeof(AfterAttribute)){
                test = x => MethodsWithAfterAndBeforeAttribute(x, obj);
            }
            else
            {
                throw new Exception("Wrong attribute type");
            }
            Parallel.ForEach(methodsWithAttribute, test);
        }

        public static object ConstuctorFinder(MethodInfo methodInfo)
        {
            var ctor = methodInfo.DeclaringType.GetConstructor(Type.EmptyTypes);
            var obj = ctor.Invoke(null);
            return obj;
        }

        public static void MethodsWithMyTestInvoker(MethodInfo methodInfo, object obj)
        {
            obj = ConstuctorFinder(methodInfo);
            MyTestAttribute attribute = (MyTestAttribute)methodInfo.GetCustomAttribute(typeof(MyTestAttribute), true);
            if (attribute.Ignore != null)
            {
                MyTests.Add(new TestStrcuct(methodInfo, isIgnored: true, ignore_message: attribute.Ignore));
            }
            else
            {
                if (attribute.Expected == null)
                {
                    MethodsInvoker<BeforeAttribute>(methodInfo.DeclaringType);
                    var watch = Stopwatch.StartNew();
                    object result = methodInfo.Invoke(obj, null);
                    watch.Stop();
                    MyTests.Add(new TestStrcuct(methodInfo, isPassed: true, timeConsumed: watch.ElapsedMilliseconds));
                    MethodsInvoker<AfterAttribute>(methodInfo.DeclaringType);
                }
                else
                {
                    try
                    {
                        MethodsInvoker<BeforeAttribute>(methodInfo.DeclaringType);
                        var watch = Stopwatch.StartNew();
                        object result = methodInfo.Invoke(obj, null);
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
        public static void MethodsWithAfterAndBeforeAttribute(MethodInfo methodInfo, object obj)
        {
            obj = ConstuctorFinder(methodInfo);
            object result = methodInfo.Invoke(obj, null);

        }

        public static void MethodsWithBeforeAndAfterClassAttribute(MethodInfo methodInfo, object obj)
        {
            if (!methodInfo.IsStatic && ((methodInfo.GetCustomAttribute(typeof(BeforeClassAttribute)) != null) || (methodInfo.GetCustomAttribute(typeof(AfterClassAttribute)) != null)))
            {
                Console.WriteLine("Wrong ch");
                throw new InvalidOperationException("Method to call must me static");
            }
            else
            {
                Console.WriteLine("Theare");
                methodInfo.Invoke(obj, null);
            }
        }
    }
}
