// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using System.Text;

namespace MyFTP;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != 4 && args.Length != 5)
        {
            Console.WriteLine("CLI expects 4 or 5 arguments in the following order: ip, port, request" +
                " and one two option:\n 1.path to file for list request\n 2. source and destanation path for Get request");
            return;
        }

        if (IPAddress.TryParse(args[0], out IPAddress? ip) && int.TryParse(args[1], out int port) &&
            int.TryParse(args[2], out int requestCode))
        {
            var ipString = args[0];
            Console.WriteLine($"port {port} and ip {ip} recognised successfully");
            ArgumentNullException.ThrowIfNull(ip);

            var client = new ClientEngine(ipString, port);
            switch (requestCode)
            {
                case 1:
                    if (args.Length != 4)
                    {
                        break;
                    }

                    var path = args[3];
                    var (size, result) = await client.ListAsync(path);
                    var resultString = new StringBuilder();
                    foreach (var file in result)
                    {
                        resultString.Append(string.Join(" ", file.Item1, file.Item2));
                        resultString.Append(' ');
                    }

                    Console.WriteLine($"Found total: {size} files and dirs, Paths: {resultString.ToString()}");
                    break;
                case 2:
                    if (args.Length != 5)
                    {
                        break;
                    }

                    var source = args[3];
                    var destanation = args[4];
                    var sizeFile = await client.GetAsync(source, destanation);
                    Console.WriteLine($"Total number of {sizeFile} copied successfully");
                    break;
                default:
                    Console.WriteLine("Command is not defined");
                    break;
            }
        }
        else
        {
            Console.WriteLine($"port or ip is not recognised");
        }
    }
}