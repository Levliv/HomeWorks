using System.Net;
using System.Text;

namespace MyFTP;

internal static class Program
{
    public static async Task Main(string[] args)
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
            string? path = args[2];
            if (path == null || ip == null)
            {
                throw new ArgumentNullException("Path should not be NULL");
            }

            var client = new ClientEngine(ipString, port);
            if (requestCode == 2)
            {
                var getResponse = await client.GetAsync(path);
                if (getResponse != null)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(getResponse.Data));

                }
                else
                {
                    Console.WriteLine("File is empty of does not exist");
                }
            }
            else if (requestCode == 1)
            {
                var resultsOfListResponse = await client.ListAsync(path);
                Console.Write(resultsOfListResponse.Count() + " ");
                foreach (var item in resultsOfListResponse)
                {
                    Console.WriteLine(item.Name + " " + item.IsDir + " ");
                }
            }
        }
        else
        {
            Console.WriteLine($"port or ip is not recognised");
        }
    }
}
