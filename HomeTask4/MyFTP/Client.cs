using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyFTP
{
    class Client
    {
        public void ClientMethod()
        {
            const int port = 8888;
            using (var client = new TcpClient("localhost", port))
            {
                Console.WriteLine($"Client Sending to port: {port}");
                var stream = client.GetStream();
                var streamWrirter = new StreamWriter(stream);
                streamWrirter.WriteLine("1 ./Tests/Files");
                streamWrirter.Flush();
                Console.WriteLine($"Client Reciving on port: {port}");
                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadToEnd();
                Console.WriteLine($"Recived data: {data}");
            }
        }
    }
}
