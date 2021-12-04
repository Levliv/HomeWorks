global using System;
using System.Net;
using System.Threading.Tasks;
namespace MyFTP;

static class Program
{
    static void Main(string[] args)
    {
        if (IPAddress.TryParse(args[0], out IPAddress? ip) && int.TryParse(args[1], out int port))
        {
            var server = new Server(ip, port);
            var task1 = Task.Run(() => server.ServerMethodAsync());
        }
        else
        {
            Console.WriteLine("Program requires two command line options");
        }
    }
}
