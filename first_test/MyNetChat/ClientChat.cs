using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace MyNetChat
{
    public class ClientChat
    {
        private IPAddress ip;
        private int port;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private Thread threadSending;
        private Thread threadRecieving;
        private List<string> messages_to_send;

        public ClientChat(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            using var client = new TcpClient(ip.ToString(), port);
            using var networkStream = client.GetStream();
            streamWriter = new StreamWriter(networkStream);
            streamReader = new StreamReader(networkStream);
        }

        public void StartUp()
        {
            var threadSending = new Thread(() =>
            {
                while (true)
                {
                    messages_to_send.Add(Console.ReadLine());
                }
            });
            while (messages_to_send.Count > 0)
            {
                streamWriter.WriteLine(messages_to_send[0]);
                messages_to_send.RemoveAt(0);
            }
            var threadRecieving = new Thread(() =>
            {
                while (true)
                {
                    string recievedMessage = streamReader.ReadLine();
                    Console.WriteLine(recievedMessage);
                }
            });

            threadSending.Start();
            threadRecieving.Start();
            threadRecieving.Join();
            threadSending.Join();
        }

        public void ShutDown()
        {
            try
            {
                threadRecieving.Abort();
                threadSending.Abort();
            }
            catch (PlatformNotSupportedException)
            {
                Console.WriteLine("ShutDown Done");
            }
        }
    }
}