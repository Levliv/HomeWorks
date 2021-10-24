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
            threads[0] = new Thread(() => server.ServerMethodAsync());
            threads[1] = new Thread(() => client.ClientMethod());
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
        }
    }
}
