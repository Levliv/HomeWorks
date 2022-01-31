using System.Net.Sockets;

namespace MyFTP;
public class ClientEngine
{
    /// <summary>
    /// Ip to listen.
    /// </summary>
    public string IpString { get; private set; }

    /// <summary>
    /// Port to listen.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Tcp Client string inforation about current connection.
    /// </summary>
    private TcpClient tcpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEngine"/> class.
    /// Constructor for the Client, creating new TCP client and connecting to the server.
    /// </summary>
    public ClientEngine(string ipString, int port)
    {
        tcpClient = new TcpClient();
        IpString = ipString;
        Port = port;
    }

    /// <summary>
    /// List request method, getting information about the files and dirictories found by the provided path.
    /// </summary>
    /// <param name="path">provided path, where to look</param>
    /// <returns>Sequence of data in base ResponseFormat</returns>
    public async Task<List<ResponseFormat>> ListAsync(string path)
    {
        if (!tcpClient.Connected)
        {
            await tcpClient.ConnectAsync(IpString, Port);
        }

        using var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        await streamWriter.WriteLineAsync(1 + " " + path);
        streamWriter.Flush();
        using var streamReader = new StreamReader(networkStream);
        var strings = streamReader.ReadLine()?.Split(" ");
        var files = new List<ResponseFormat>();
        if (strings == null)
        {
            return files;
        }

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
    public async Task<GetResponseStruct> GetAsync(string path)
    {
        if (!tcpClient.Connected)
        {
            await tcpClient.ConnectAsync(IpString, Port);
        }

        using var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        await streamWriter.WriteLineAsync(2 + " " + path);
        streamWriter.Flush();
        var streamReader = new StreamReader(networkStream);
        var messageLength = int.Parse(streamReader.ReadLine() ?? "0");
        using var streamBinaryReader = new BinaryReader(networkStream);
        var bytes = streamBinaryReader.ReadBytes(messageLength);
        return new GetResponseStruct(bytes);
    }
}
