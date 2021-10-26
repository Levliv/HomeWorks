using System.Threading.Tasks;
using System;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var client = new Client("1 ./Tests/Files");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
        }
    }
}
