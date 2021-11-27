using System.Net.Sockets;
using System.Text;

namespace MyFTPClient;

/// <summary>
/// Get and List Requests Enumeration
/// </summary>
public enum RequestType
{
    Get,
    List
}

/// <summary>
/// Class for Client of tcp protocol
/// </summary>
public class Client
{
    /// <summary>
    /// Type of request from the enum: Get or List
    /// </summary>
    public RequestType FtpRequestType { get; private set; }

    /// <summary>
    /// Ip to listen
    /// </summary>
    public string IpString { get; private set; }

    /// <summary>
    /// Port to listen
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Tcp Client string inforation about current connection
    /// </summary>
    public TcpClient TcpClient { get; private set; }
    
    /// <summary>
    /// Structs to preserve information about files in directories got from Server by List Request
    /// </summary>

    public IEnumerable<ResponseFormat>? ResultsOfListResponse { get; private set; }

    /// <summary>
    /// Containing the information about the stream of the current connection
    /// </summary>

    public NetworkStream? MyStreamReader { get; private set; }

    /// <summary>
    /// Structs to preserve information about file got from Server by Get Request
    /// </summary>

    public GetResponseStruct GetResponse { get; private set; }

    /// <summary>
    /// Constructor for the Client, creating new tcp client and connecting to the server
    /// </summary>

    public Client(string ipString, int port)
    {
        TcpClient = new TcpClient(ipString, port);
        IpString = ipString;
        Port = port;
    }

    /// <summary>
    /// Sending requests to Server by invoking Get/List Client Methods
    /// </summary>
    public void ClientRequest(string requestString)
    {
        var splitedRequestString = requestString.Split(" ");
        var request = splitedRequestString[0];
        var path = splitedRequestString[1];
        if (request == "2")
        {
            FtpRequestType = RequestType.Get;
        }
        else
        if (request == "1")
        {
            FtpRequestType = RequestType.List;
        }
        switch (FtpRequestType)
        {
            case RequestType.Get:
                GetResponse = Get(path);
                break;
            case RequestType.List:
                ResultsOfListResponse = List(path);
                break;
        }
    }

    /// <summary>
    /// Printing the results got by tcp
    /// </summary>
    public void PrintResults()
    {
        switch (FtpRequestType)
        {
            case RequestType.Get:
                Console.WriteLine(Encoding.UTF8.GetString(GetResponse.Data));
                break;
            case RequestType.List:
                Console.Write(ResultsOfListResponse.Count() + " ");
                foreach (var item in ResultsOfListResponse)
                {
                    Console.WriteLine(item.Name + " " + item.IsDir + " ");
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// List request method, getting information about the files and dirictories found by the provided path  
    /// </summary>
    /// <param name="path">provided path, where to look</param>
    /// <returns>Sequence of data in base ResponseFormat</returns>
    public IEnumerable<ResponseFormat> List(string path)
    {
        using var networkStream = TcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        streamWriter.WriteLine(1 + " " + path);
        streamWriter.Flush();
        using var streamReader = new StreamReader(networkStream);
        var strings = streamReader.ReadLine().Split(" ");
        var files = new List<ResponseFormat>();
        for (var i = 1; i < int.Parse(strings[0]) * 2; i+=2)
        {
            files.Add(new ResponseFormat(strings[i], strings[i+1]));
        }
        return files;
    }

    /// <summary>
    /// Getting the file data, stroing in the current file, by provided path
    /// </summary>
    /// <param name="path">provided relative path</param>
    /// <returns> Base struct GetResponseStruct</returns>
    public GetResponseStruct Get(string path)
    {
        using var networkStream = TcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        streamWriter.WriteLine(2 + " " + path);
        streamWriter.Flush();
        MyStreamReader = TcpClient.GetStream();
        using var streamReader = new StreamReader(MyStreamReader);
        var messageLenght = int.Parse(streamReader.ReadLine());
        using var streamBinaryReader = new BinaryReader(MyStreamReader);
        var bytes = streamBinaryReader.ReadBytes(messageLenght);
        var result = new GetResponseStruct(messageLenght, bytes);
        return result;
    }
}
