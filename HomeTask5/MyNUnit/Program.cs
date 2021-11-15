using System;
using System.Reflection;
using System.Collections.Generic;

namespace MyNUnit
{
    class Student
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public int Age { get; set; }

        [MyTest]
        public bool Expel()
        {
            //Console.WriteLine("Expelled successfully");
            Name = null;
            SurName = null;
            Age = 0;
            return true;
        }

        [MyTest(Expected = typeof(ArgumentException), Ignore = "WTF")]
        public int Expel2()
        {
            //Console.WriteLine("Expelled successfully 2");
            Name = null;
            SurName = null;
            Age = 0;
            //throw new ArgumentException();
            return 13;
        }
        private void ChangeStudent(string NewStudentName, string NewStudentSurname, int NewStudentAge)
        {
            Name = NewStudentName;
            SurName = NewStudentSurname;
            Age = NewStudentAge;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Type type = Type.GetType("MyNUnit.Student", true, true);
            Student student = new Student();
            var methods = type.GetMethods();
            var MethodsWithMyTestList = new List<MethodInfo> { };
            var MethodsWithMyBeforeTestList = new List<MethodInfo> { };
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attributesMyTest = methodInfo.GetCustomAttributes(typeof(MyTestAttribute), true);
                var attributesMyBeforeTest = methodInfo.GetCustomAttributes(typeof(BeforeAttribute), false);
                if (attributesMyTest.Length > 0)
                {
                    Console.WriteLine($"{methodInfo} Attributes:");
                    foreach (MyTestAttribute attribute in attributesMyTest)
                    {
                        if (attribute.Ignore != null)
                        {
                            Console.WriteLine($"Test with {attribute} for method {methodInfo.Name} wasn't called. Description: {attribute.Ignore}");
                        }
                        else
                        {
                            if (attribute.Expected == null)
                            {
                                Console.WriteLine("Ok");
                            }
                            else
                            {
                                try
                                {
                                    object result = type.InvokeMember(methodInfo.Name, BindingFlags.InvokeMethod, null, student, new object[] { });
                                    if (attribute.Expected.Equals(result))
                                    {
                                        Console.WriteLine("Ok");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error");
                                        Console.WriteLine($"Expected: {attribute.Expected}, but got {result}");
                                    }
                                }
                                catch (Exception exception)
                                {
                                    if (attribute.Expected.Equals(exception.InnerException.GetType()))
                                    {
                                        Console.WriteLine("Ok");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Not Ok");
                                        Console.WriteLine($"Expected: {attribute.Expected}, but got {exception}");
                                    }
                                }
                            }
                        }
                    Console.WriteLine("Check 2");
                    Console.WriteLine(methodInfo.Name);
                    }
                }

                if (attributesMyTest.Length > 0)
                {
                    MethodsWithMyTestList.Add(methodInfo);
                }
                if (attributesMyBeforeTest.Length > 0)
                {
                    MethodsWithMyBeforeTestList.Add(methodInfo);
                }
            }

            foreach (var methodInfo in MethodsWithMyTestList)
            {
            }
        }
    }
}
