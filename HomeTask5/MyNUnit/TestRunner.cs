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
                foreach (var type in types)
                {
                    Console.WriteLine($"======{type.Name}==========");
                    var methods = type.GetMethods();
                    foreach (var methodInfo in methods)
                    {
                        var attributesMyTest = methodInfo.GetCustomAttributes(typeof(MyTestAttribute), true);
                        var attributesBeforeTest = methodInfo.GetCustomAttributes(typeof(BeforeAttribute), true);
                        var attributesAfterTest = methodInfo.GetCustomAttributes(typeof(AfterAttribute), true);
                        if (attributesMyTest.Length > 0)
                        {
                            foreach (MyTestAttribute attribute in attributesMyTest) 
                            {
                                MyTests.Add(new TestStrcuct(methodInfo, assembly, type));
                            }
                            Action<MethodInfo> MethodInvoker = ;
                            Parallel.ForEach(MyTests, MethodInvoker);
                        }
                    }
                }
            }
        }
        public static void TestInvoker<AttributeType>(Type type)
        {

        }
    }
}
