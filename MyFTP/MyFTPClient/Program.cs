using System.Net;
namespace MyFTPClient;

static class Program
{
    static void Main(string[] args)
    {
        if (int.TryParse(args[1], out int port) && IPAddress.TryParse(args[0], out IPAddress? ip))
        {
            string ipString = args[0];
            Console.WriteLine($"port {port} and ip {ip} recognised successfully");
            while (true)
            {
                string? request = args[2];
                if (request == null || ip == null)
                {
                    throw new ArgumentNullException("Request and ip should not be NULL");
                }
                var client = new Client(ipString, port);
                client.ClientRequest(request);
                client.PrintResults();
            }
        }
        else
        {
            Console.WriteLine($"port or ip is not recognised");
        }
    }
}
