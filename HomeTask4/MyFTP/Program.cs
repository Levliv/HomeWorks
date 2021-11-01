using System.Threading.Tasks;
using System;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var client = new Client("3 ./Tests/Files/TestBile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Console.WriteLine("Prog WriteLine");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(client.ReceivedData));
        }
    }
}
