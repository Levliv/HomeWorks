namespace MyNUnit;

using System;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Not enought args has been entered, need: 1");
            return;
        }

        var path = args[0];
        try
        {
            TestRunner.Start(path);
            TestRunner.PrintTestResults();
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"Path: {path} is invaild, couldn't file the file");
        }
        catch (FieldAccessException)
        {
            Console.WriteLine($"You have no access to this file");
        }
        catch (FileLoadException)
        {
            Console.WriteLine("An error ocures while dll files loading");
        }
    }
}
