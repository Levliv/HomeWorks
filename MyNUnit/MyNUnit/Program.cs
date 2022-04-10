using MyNUnit;

if (args.Length != 1)
{
    Console.WriteLine("Wrong number of args has been entered, need: 1");
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
