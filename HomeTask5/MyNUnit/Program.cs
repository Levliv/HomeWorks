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
            Console.WriteLine("Expelled successfully");
            Name = null;
            SurName = null;
            Age = 0;
            return true;
        }

        [MyTest(Expected = 12, Ignore = "WTF")]
        public bool Expel2()
        {
            Console.WriteLine("Expelled successfully 2");
            Name = null;
            SurName = null;
            Age = 0;
            return true;
        }
        private void ChangeStudent(string NewStudentName, string NewStudentSurname, int NewStudentAge)
        {
            Name = NewStudentName;
            SurName = NewStudentSurname;
            Age = NewStudentAge;
        }
    }

    public class MyBeforeTestAttribute : Attribute
    {

    }
    public class MyTestAttribute: Attribute
    {
        public int Expected { get; set; }
        public string Ignore { get; set; }
        public MyTestAttribute() { }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Type type = Type.GetType("MyNUnit.Student", false, true);
            Student student = new Student();
            var methods = type.GetMethods();
            var MethodsWithMyTestList = new List<MethodInfo> { };
            var MethodsWithMyBeforeTestList = new List<MethodInfo> { };
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attributesMyTest = methodInfo.GetCustomAttributes(typeof(MyTestAttribute), true);
                var attributesMyBeforeTest = methodInfo.GetCustomAttributes(typeof(MyBeforeTestAttribute), false);
                if (attributesMyTest.Length > 0)
                {
                    foreach (object attribute in attributesMyTest)
                    {
                        Console.WriteLine(attribute.ToString());
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
                type.InvokeMember(methodInfo.Name, BindingFlags.InvokeMethod, null, student, new object[] { });
                Console.WriteLine(methodInfo.Name);
            }
        }
    }
}
