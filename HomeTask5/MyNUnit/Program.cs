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
        /// <summary>
        /// Number of passed tests
        /// </summary>
        static public int Passed { get; private set; }

        /// <summary>
        /// Number of skiped tests
        /// </summary>
        static public int Skiped { get; private set; }

        /// <summary>
        /// Number of not passed tests
        /// </summary>
        static public int Failed { get; private set; }

        static void Main(string[] args)
        {
            TestRunner.Start(Console.ReadLine());
            /*
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
                        switch (MethodInvoked(attribute, methodInfo, type, student))
                        {
                            case 1:
                                Skiped++;
                                break;
                            case 2:
                                Passed++;
                                break;
                            case -1:
                                Failed++;
                                break;
                        }
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
            */
        }
        /// <summary>
        /// TestMethod Invoked. Returns:
        /// 1 - for Skipped
        /// 2 - for Passed
        /// -1 - for Failed
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="methodInfo"></param>
        /// <param name="type"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        static int MethodInvoked(MyTestAttribute attribute, MethodInfo methodInfo, Type type, Student student)
        {
            if (attribute.Ignore != null)
            {
                Console.WriteLine($"Test with {attribute} for method {methodInfo.Name} wasn't called. Description: {attribute.Ignore}");
                return 1;
            }
            else
            {
                if (attribute.Expected == null)
                {
                    return 2;
                }
                else
                {
                    try
                    {
                        object result = type.InvokeMember(methodInfo.Name, BindingFlags.InvokeMethod, null, student, new object[] { });
                        if (attribute.Expected.Equals(result))
                        {
                            return 2;
                        }
                        else
                        {
                            Console.WriteLine("Error");
                            Console.WriteLine($"Expected: {attribute.Expected}, but got {result}");
                            return -1;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (attribute.Expected.Equals(exception.InnerException.GetType()))
                        {
                            return 2;
                        }
                        else
                        {
                            Console.WriteLine("Not Ok");
                            Console.WriteLine($"Expected: {attribute.Expected}, but got {exception}");
                            return -1;
                        }
                    }
                }
            }
        }
    }
}
