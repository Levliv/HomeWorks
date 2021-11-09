using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

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
        public string ReceivedData { get; private set; }
        public byte[] FileData { get; private set; }

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

        public List<RespondFileStuct> List(string path, NetworkStream networkStream)
        {
            using var streamWrirter = new StreamWriter(networkStream);
            streamWrirter.WriteLine(path);
            streamWrirter.Flush();
            return new List<RespondFileStuct>() { };
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
            //if (path[0] == 1)
            //{
            //    List("", networkStream);
            //}
            //else
            //{
                using var streamReader = new BinaryReader(networkStream);
                FileData = streamReader.ReadBytes(65535);
            //}
        }
    }
}