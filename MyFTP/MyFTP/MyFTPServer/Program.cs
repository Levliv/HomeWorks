// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using MyFTP;

if (args.Length == 2 && IPAddress.TryParse(args[0], out IPAddress? ip) && int.TryParse(args[1], out int port))
{
    var server = new ServerEngine(ip, port);
    var task = server.Run();
    var command = "";
    while (command != "exit")
    {
        Console.WriteLine("To stop server write: \"exit\"");
        command = Console.ReadLine();
    }

    server.Cts.Cancel();
    await task;
}
else
{
    Console.WriteLine("Program requires two command line options, use them in the following order: ip, port");
}