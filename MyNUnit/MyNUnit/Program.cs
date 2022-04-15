using MyNUnit;

if (args.Length != 1)
{
    Console.WriteLine("Wrong number of args has been entered, need: 1\n" +
        "Enter path to the directory with dll files and try again");
    return;
}

var path = args[0];
try
{
    var testRunner = new TestRunner();
    testRunner.Start(path);
    testRunner.PrintTestResults();
}
catch (DirectoryNotFoundException)
{
    Console.WriteLine($"Path: {path} is invalid, directory wasn't found");
}
catch (FieldAccessException)
{
    Console.WriteLine($"You have no access to this file");
}
catch (FileLoadException)
{
    Console.WriteLine("An error occured while dll files were loading");
}
