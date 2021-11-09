using System;
using System.Net;

namespace MyNetChat
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip;
            int port;
            if (args.Length == 1 && int.TryParse(args[0], out port))
            {
                var server = new ServerChat(port);
                server.StartUp();
            }
            else
            {
                if (args.Length == 2 && IPAddress.TryParse(args[0], out ip) && int.TryParse(args[1], out port))
                {
                    var client = new ClientChat(ip, port);
                    client.StartUp();
                }
                else
                {
                    Console.WriteLine($"Not correct: number of params {args.Length}, need: 1 or 2");
                }
            }
        }
    }
}
