using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyFTP
{
    class Server: IServer
    {
        public (int size, string name, bool isDir) List(string path)
        {
            return (3, "sad", false);
        }
        public (int size, byte[] content) Get(string path)
        {
            return (4, Encoding.Unicode.GetBytes("sad boy"));
        }
        public static void ServerMethod()
        {
            Console.WriteLine("Starting");
            const int port = 8888;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Listening on port: {port}");
            using (var socket = listener.AcceptSocket())
            {
                var stream = new NetworkStream(socket);
                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadLine();
                Console.WriteLine($"Recived: {data}");

                Console.WriteLine($"Sending \"Hi!\"");
                var streamWriter = new StreamWriter(stream);
                streamWriter.WriteLine("Hi!");
                streamWriter.Flush();
            }
            listener.Stop();
            Console.WriteLine("Finishing");
        }
        public static void ClientMethod()
        {
            const int port = 8888;
            using (var client = new TcpClient("localhost", port))
            {
                Console.WriteLine($"Sending to port: {port}");
                var stream = client.GetStream();
                var streamWrirter = new StreamWriter(stream);
                streamWrirter.WriteLine("Hello World!");
                streamWrirter.Flush();

                Console.WriteLine($"Reciving on port: {port}");
                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadLine();
                Console.WriteLine($"Recived data: {data}");
            }
        }
    }
}
