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
            var server = new Server();
            var client = new Client();
            server.ServerMethodAsync();
            client.ClientMethod();
        }
    }
}
