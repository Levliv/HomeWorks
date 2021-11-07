using System.Net;
using System.Threading.Tasks;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip;
            int port;
            if (IPAddress.TryParse(args[0], out ip) && int.TryParse(args[1], out port))
            {
                var server = new Server(ip, port);
                var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            }
        }
    }
}
