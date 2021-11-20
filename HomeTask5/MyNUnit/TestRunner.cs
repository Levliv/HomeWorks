using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace MyNUnit
{
    public static class TestRunner
    {
        public static BlockingCollection<TestStrcuct> MyTests { get; private set; }

        public static void Start(string path)
        {
            var dllfiles = Directory.GetFiles(path,"*.dll", SearchOption.AllDirectories);
            foreach(var dll in dllfiles)
            {
                var assembly = Assembly.LoadFrom(dll);
                var types = assembly.GetTypes();
                Parallel.ForEach(types, TestStarter);
            }
        }
        public static void TestStarter(Type type)
        {
            Console.WriteLine($"======{type.Name}==========");

            MethodsInvoker<BeforeClassAttribute>(type);
            MethodsInvoker<MyTestAttribute>(type);
            MethodsInvoker<AfterClassAttribute>(type);
        }
        public static void MethodsInvoker<AttributeType>(Type type) // where selector Надо убедитья, что AttributeType наследник Attribute
        {
            var methodsinfo = type.GetTypeInfo().DeclaredMethods;
            foreach (var methodInfo in methodsinfo)
            {
                var methodsWithAttribute = methodInfo.GetCustomAttributes(typeof(AttributeType), true);
            }
        }
        static void MethodsWithMyTestInvoker(MethodInfo methodInfo, Student student)
        {
            MyTestAttribute attribute = (MyTestAttribute)methodInfo.GetCustomAttribute(typeof(MyTestAttribute), true);
            if (attribute.Ignore != null)
            {
                Console.WriteLine($"Test with {typeof(MyTestAttribute).Name} for method {methodInfo.Name} wasn't called. Description: {attribute.Ignore}");
                MyTests.Add(new TestStrcuct(methodInfo, isIgnored: true, ignore_message: attribute.Ignore));
            }
            else
            {
                if (attribute.Expected == null)
                {
                    MyTests.Add(new TestStrcuct(methodInfo, isPassed: true));
                }
                else
                {
                    try
                    {
                        object result = methodInfo.Invoke(student, null);
                        if (attribute.Expected.Equals(result))
                        {
                            MyTests.Add(new TestStrcuct(methodInfo, isPassed: true)); ;
                        }
                        else
                        {
                            Console.WriteLine("Error");
                            Console.WriteLine($"Expected: {attribute.Expected}, but got {result}");
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
                            Console.WriteLine("Not Ok");
                            Console.WriteLine($"Expected: {attribute.Expected}, but got {exception}");
                            MyTests.Add(new TestStrcuct(methodInfo, isFailed: true));
                        }
                    }
                }
            }
        }

        static void MethodsWithBeforeAndAfterAttribute(MethodInfo methodInfo, object obj)
        {
            if (methodInfo.IsStatic && ((methodInfo.GetCustomAttribute(typeof(BeforeClassAttribute)) != null) || (methodInfo.GetCustomAttribute(typeof(AfterClassAttribute)) != null)))
            {
                methodInfo.Invoke(obj, null);
            }
            else
            {
                throw new InvalidOperationException("Method to call must me static");
            }
        }
    }
}
