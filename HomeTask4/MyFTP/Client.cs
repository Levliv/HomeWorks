using System;
using System.IO;
using System.Net.Sockets;

namespace MyFTP
{
    public class Client
    {
        private string path;

        /// <summary>
        /// Datagram for recived data
        /// </summary>
        public byte[] ReceivedData { get; private set; }
        
        /// <summary>
        /// Construxtor for client
        /// </summary>
        /// <param name="request">Request for the server</param>
        public Client(string request)
        {
            path = request;
        }

        /// <summary>
        /// Method to send requests and get responds as a client
        /// </summary>
        public void ClientMethod()
        {
            const int port = 8888;
            using (var client = new TcpClient("localhost", port))
            {
                var stream = client.GetStream();
                var streamWrirter = new StreamWriter(stream);
                streamWrirter.WriteLine(path);
                streamWrirter.Flush();
                try
                {
                    var streamReader = new BinaryReader(stream);
                    ReceivedData = streamReader.ReadBytes(65535);
                    Console.WriteLine("Try");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Catch client");
                    throw ex;
                }
                //Console.WriteLine(System.Text.Encoding.UTF8.GetString(ReceivedData));
            }
        }
    }
}
