using System.Net.Sockets;

namespace MyFTPClient;
public enum RequestType
{
    Get,
    List
}

public class Client
{
    public RequestType RequestType { get; private set; }
    public string IpString { get; private set; }
    public int Port { get; private set; }
    public TcpClient TcpClient { get; private set; }

    public IEnumerable<ResponseFormat>? ResultsOfListResponse { get; private set; }

    public NetworkStream? MyStreamReader { get; private set; }

    public Client(string ipString, int port)
    {
        using var client = new TcpClient(ipString, port);
        IpString = ipString;
        Port = port;
    }

    public void ClientRequest(string request, string path)
    {
        if (request == "Get")
        {
            RequestType = RequestType.Get;
        }
        else
        if (request == "List")
        {
            RequestType = RequestType.List;
        }
        switch (RequestType)
        {
            case RequestType.Get:
                MyStreamReader = Get();
                break;
            case RequestType.List:
                ResultsOfListResponse = List(path);
                break;
        }
    }

    public void PrintResults()
    {
        switch (RequestType)
        {
            case RequestType.Get:
                Console.Write(MyStreamReader);
                break;
            case RequestType.List:
                Console.Write(ResultsOfListResponse.Count() + " ");
                foreach(var item in ResultsOfListResponse)
                    Console.WriteLine(item.Name + " " + item.IsDir + " ");
                break;
            default:
                break;
        }
    }

    public IEnumerable<ResponseFormat> List(string path)
    {
        using var networkStream = TcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        streamWriter.WriteLine(RequestType + path);
        streamWriter.Flush();
        using var streamReader = new StreamReader(networkStream);
        var strings = streamReader.ReadToEnd().Split(" ");
        var files = new List<ResponseFormat>();
        for (var i = 1; i < int.Parse(strings[i]) * 2; i+=2)
        {
            files.Add(new ResponseFormat(strings[i], strings[i+1])); // Here can be a mistake with a false/true recognision
        }
        return files;
    }

    public NetworkStream Get()
    {
        return TcpClient.GetStream();
    }
}
