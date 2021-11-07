using System;
using System.Net;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            int port;
            IPAddress ip;
            string request;
            if (int.TryParse(args[1], out port) && IPAddress.TryParse(args[0], out ip))
            {
                Console.WriteLine($"port {port} and ip {ip} recognised successfully");
                while (true)
                {
                    Console.WriteLine("Enter your requset, master");
                    request = Console.ReadLine();
                    var client = new Client(request, ip, 8888);
                    client.ClientMethod();
                    Console.WriteLine(client.ReceivedData);
                }
            }
            else
            {
                Console.WriteLine($"port or ip is not recognised");
            }
        }
    }
}
