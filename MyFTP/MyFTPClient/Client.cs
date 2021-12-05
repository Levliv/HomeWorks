using System.Net.Sockets;

namespace MyFTPClient;


/// <summary>
/// Class for Client of TCP protocol
/// </summary>
public class Client
{
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
    private TcpClient TcpClient;

    /// <summary>
    /// Structs to preserve information about files in directories got from Server by List Request
    /// </summary>
    private IEnumerable<ResponseFormat>? ResultsOfListResponse;

    /// <summary>
    /// Containing the information about the stream of the current connection
    /// </summary>
    private NetworkStream? MyStreamReader;

    /// <summary>
    /// Constructor for the Client, creating new TCP client and connecting to the server
    /// </summary>
    public Client(string ipString, int port)
    {
        TcpClient = new TcpClient();
        IpString = ipString;
        Port = port;
    }

    /// <summary>
    /// List request method, getting information about the files and dirictories found by the provided path  
    /// </summary>
    /// <param name="path">provided path, where to look</param>
    /// <returns>Sequence of data in base ResponseFormat</returns>
    public IEnumerable<ResponseFormat> List(string path)
    {
        if (!TcpClient.Connected)
        {
            TcpClient.Connect(IpString, Port);
        }
        var networkStream = TcpClient.GetStream();
        var streamWriter = new StreamWriter(networkStream);
        streamWriter.WriteLine(1 + " " + path);
        streamWriter.Flush();
        var streamReader = new StreamReader(networkStream);
        var strings = streamReader.ReadLine().Split(" ");
        var files = new List<ResponseFormat>();
        for (var i = 1; i < int.Parse(strings[0]) * 2; i += 2)
        {
            files.Add(new ResponseFormat(strings[i], strings[i + 1]));
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
        TcpClient.Connect(IpString, Port);
        using var networkStream = TcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        streamWriter.WriteLine(2 + " " + path);
        streamWriter.Flush();
        MyStreamReader = TcpClient.GetStream();
        var streamReader = new StreamReader(MyStreamReader);
        var messageLength = int.Parse(streamReader.ReadLine());
        using var streamBinaryReader = new BinaryReader(MyStreamReader);
        var bytes = streamBinaryReader.ReadBytes(messageLength);
        var result = new GetResponseStruct(messageLength, bytes);
        return result;
    }
}
