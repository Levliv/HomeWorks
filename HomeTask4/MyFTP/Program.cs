using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            var threads = new Thread[2];
            var server = new Server();
            var client = new Client();
            var task1 = Task.Run(() => server.ServerMethodAsync());
            var task2 = Task.Run(() => client.ClientMethod());
            task1.Wait();
            task2.Wait();
        }
    }
}
