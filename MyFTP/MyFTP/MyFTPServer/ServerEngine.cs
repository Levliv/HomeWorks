using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyFTP;

/// <summary>
/// Base Server class.
/// </summary>
public class ServerEngine
{
    /// <summary>
    /// Gets path to the base directory where the search is supposed to begin.
    /// </summary>
    public string DataPath { get; private set; }

    /// <summary>
    /// Gets port for the server.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Gets ip of the server.
    /// </summary>
    public IPAddress Ip { get; private set; }

    /// <summary>
    /// Gets or sets to stop the server, all requests recieved before cancellation will be processed.
    /// </summary>
    public CancellationTokenSource Cts { get; set; } = new ();

    private readonly TcpListener listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerEngine"/> class.
    /// Constructor for Server.
    /// </summary>
    /// <param name="dirBackPath">Path to the base directory where the search is supposed to begin</param>
    public ServerEngine(IPAddress ip, int port, string dirBackPath = "../../../.")
    {
        DataPath = dirBackPath;
        Port = port;
        Ip = ip;
        listener = new TcpListener(Ip, Port);
    }

    /// <summary>
    /// Start serving clients
    /// </summary>
    public async Task Run()
    {
        listener.Start();
        while (!Cts.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            Task.Run(() => ServerMethod(socket));
        }
        listener.Stop();
    }

    private async Task ServerMethod(Socket socket)
    {
        using var networkStream = new NetworkStream(socket);
        using var streamReader = new StreamReader(networkStream);
        var t = streamReader.ReadLineAsync();
        var data = await t;
        var strings = data.Split(' ');
        var requestPath = strings[1];
        switch (int.Parse(strings[0]))
        {
            case 1: // List request Case
                {
                    using var streamWriter = new StreamWriter(networkStream);
                    streamWriter.WriteLine(await List(DataPath + requestPath));
                    streamWriter.Flush();
                    break;
                }
            case 2: // Get request Case
                {
                    var (size, bytes) = Get(DataPath + requestPath);
                    using var streamWriter = new StreamWriter(networkStream);
                    var sizeOfMessage = bytes.Length;
                    streamWriter.WriteLine(sizeOfMessage);
                    streamWriter.Flush();
                    using var streamBinaryWriter = new BinaryWriter(networkStream);
                    streamBinaryWriter.Write(bytes);
                    streamBinaryWriter.Flush();
                    break;
                }
        }
    }

    /// <summary>
    /// Returns datagram in respond of Get request
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Size of file, massive of bytes(file)</returns>
    public (long size, byte[] content) Get(string path)
    {
        if (File.Exists(path))
        {
            var dataBytes = File.ReadAllBytes(path);
            return (dataBytes.Length, dataBytes);
        }
        return (-1, new byte[0]);
    }

    /// <summary>
    /// Creates server response
    /// </summary>
    /// <param name="path">path to the directory we need to look at</param>
    /// <returns>srting in format string with server respond</returns>
    public async Task<string> List(string path)
    {
        var (size, name) = await ListProсess(path);
        return size != -1 ? $"{size} {name}" : "-1";
    }

    private async Task<(int, string)> ProсessFiles(FileInfo[] files)
    {
        var stringBuilder = new StringBuilder();
        var dir = Path.GetFullPath(DataPath + ".");
        foreach (var file in files)
        {
            stringBuilder.Append("." + file.ToString().Replace(dir, string.Empty).Replace('\\', '/') + " false");
        }
        var resultString = stringBuilder.ToString();
        return (files.Length, resultString);
    }

    private async Task<(int, string)> ProсessDirectories(DirectoryInfo[] directories)
    {
        var stringBuilder = new StringBuilder();
        var dir = Path.GetFullPath(DataPath + ".");
        foreach (var directory in directories)
        {
            stringBuilder.Append("." + directory.ToString().Replace(dir, string.Empty).Replace('\\', '/') + " true");
        }
        var resultString = stringBuilder.ToString();
        return (directories.Length, resultString);
    }

    /// <summary>
    /// Server's method for seraching in order to create list of files and dirs in the directory
    /// </summary>
    public async Task<(int size, string name)> ListProсess(string path)
    {
        var di = new DirectoryInfo(path);
        if (di.Exists)
        {
            var (numberOfFiles, strFiles) = await ProсessFiles(di.GetFiles());
            var (numberOfDirectories, strDirs) = await ProсessDirectories(di.GetDirectories());
            return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs);
        }
        return (-1, string.Empty);
    }
}
