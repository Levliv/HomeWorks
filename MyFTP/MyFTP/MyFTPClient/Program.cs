using System.Net;
using System.Text;

namespace MyFTP;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine($"CLI expects 3 arguments in the following order: ip, port, request ");
            return;
        }

        if (int.TryParse(args[1], out int port) && IPAddress.TryParse(args[0], out IPAddress? ip) && int.TryParse(args[2], out int requestCode))
        {
            string ipString = args[0];
            Console.WriteLine($"port {port} and ip {ip} recognised successfully");
            while (true)
            {
                string? path = args[3];
                if (path == null || ip == null)
                {
                    throw new ArgumentNullException("Path should not be NULL");
                }

                var client = new ClientEngine(ipString, port);
                if (requestCode == 2)
                {
                    var GetResponse = client.Get(path);
                    var t = GetResponse.Result;
                    Console.WriteLine(Encoding.UTF8.GetString(t.Data));
                }
                else if (requestCode == 1)
                {
                    var resultsOfListResponse = client.List(path);
                    Console.Write(resultsOfListResponse.Count() + " ");
                    foreach (var item in resultsOfListResponse)
                    {
                        Console.WriteLine(item.Name + " " + item.IsDir + " ");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine($"port or ip is not recognised");
        }
    }
}
