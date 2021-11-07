using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace MyFTP
{

    /// <summary>
    /// Client for interrogation server
    /// </summary>
    public class Client
    {
        private string path;
        private IPAddress ip;
        private int port;

        /// <summary>
        /// Datagram for recieved data
        /// </summary>
        public byte[] ReceivedData { get; private set; }

        /// <summary>
        /// Constructor for client
        /// </summary>
        /// <param name="request">Request for the server</param>
        public Client(string request, IPAddress ip, int port)
        {
            path = request;
            this.ip = ip;
            this.port = port;
        }

        public void List(string path)
        {
            var obj = new RespondFileStuct.RespondFileStuct();
            return;
        }

        /// <summary>
        /// Method to send requests and get responds as a client
        /// </summary>
        public void ClientMethod()
        {
            using var client = new TcpClient("127.0.0.1", port);
            using var networkStream = client.GetStream();
            using var streamWrirter = new StreamWriter(networkStream);
            streamWrirter.WriteLine(path);
            streamWrirter.Flush();
            using var streamReader = new BinaryReader(networkStream);
            ReceivedData = streamReader.ReadBytes(65535);
        }
    }
}