using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyFTP
{
    public enum RequestType
    {
        Get,
        List
    }

    public class Client
    {
        private RequestType requestType;
        private string ipString;
        private int port;

        public IEnumerable<ResponseFormat>? ResultsOfListResponse { get; private set; }

        public StreamReader? MyStreamReader { get; private set; }

        public Client(string ipString, int port)
        {
            using var client = new TcpClient(ipString, port);
            this.ipString = ipString;
            this.port = port;
        }

        public void ClientRequest(string request)
        {
            if (request == "Get")
            {
                requestType = RequestType.Get;
            }
            else
            if (request == "List")
            {
                requestType = RequestType.List;
            }
            switch (requestType)
            {
                case RequestType.Get:
                    MyStreamReader = Get();
                    break;
                case RequestType.List:
                    ResultsOfListResponse = List();
                    break;
            }
        }

        public void PrintResults()
        {
            switch (requestType)
            {
                case RequestType.Get:
                    Console.Write(MyStreamReader);
                    break;
                case RequestType.List:
                    Console.Write(ResultsOfListResponse.Count() + " ");
                    foreach (var item in ResultsOfListResponse)
                        Console.WriteLine(item.Name + " " + item.IsDir + " ");
                    break;
                default:
                    break;
            }
        }

        public IEnumerable<ResponseFormat> List()
        {
            return new List<ResponseFormat>();
        }

        public StreamReader Get()
        {
            return new StreamReader("123");
        }
    }
}
