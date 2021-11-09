using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MyNetChat
{
    public class ServerChat
    {
        private int port;
        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private NetworkStream netstream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private Thread threadSending;
        private Thread threadRecieving;
        private List<string> messages_to_send;

        public ServerChat(int port)
        {
            this.port = port;
            var listener = new TcpListener(ip, port);
            listener.Start();
            using var socket = listener.AcceptSocket();
            netstream = new NetworkStream(socket);
            streamReader = new StreamReader(netstream);
            streamWriter = new StreamWriter(netstream);
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
