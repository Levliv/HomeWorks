using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyFTP;

/// <summary>
/// Server class to process client's requests
/// </summary>
public class Server
{
    /// <summary>
    /// Path to the base directory where the search is supposed to begin
    /// </summary>
    public string DataPath { get; private set; }

    /// <summary>
    /// Port for the server
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Ip of the server
    /// </summary>
    public IPAddress Ip { get; private set; }

    /// <summary>
    /// Constructor for Server
    /// </summary>
    /// <param name="dirBackPath">Path to the base directory where the search is supposed to begin</param>
    public Server(IPAddress ip, int port, string dirBackPath = "../../../.")
    {
        DataPath = dirBackPath;
        Port = port;
        Ip = ip;
    }

    private (int, string) ProsessFiles(FileInfo[] files)
    {
        var stringBuilder = new StringBuilder();
        var dir = Path.GetFullPath(DataPath + ".");
        foreach (var file in files)
        {
            stringBuilder.Append("." + file.ToString().Replace(dir, "").Replace('\\', '/') + " false");
        }
        var resultString = stringBuilder.ToString();
        return (files.Length, resultString);
    }

    private (int, string) ProsessDirectories(DirectoryInfo[] directories)
    {
        var stringBuilder = new StringBuilder();
        var dir = Path.GetFullPath(DataPath + ".");
        foreach (var directory in directories)
        {
            stringBuilder.Append("." + directory.ToString().Replace(dir, "").Replace('\\', '/') + " true");
        }
        var ResultString = stringBuilder.ToString();
        return (directories.Length, ResultString);
    }

    /// <summary>
    /// Server's method for seraching in order to create list of files and dirs in the directory
    /// </summary>
    public (int size, string name) ListProsess(string path)
    {
        var di = new DirectoryInfo(path);
        if (di.Exists)
        {
            var (numberOfFiles, strFiles) = ProsessFiles(di.GetFiles());
            var (numberOfDirectories, strDirs) = ProsessDirectories(di.GetDirectories());
            return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs);
        }
        return (-1, "");
    }

    /// <summary>
    /// Creates server respond
    /// </summary>
    /// <param name="path">path to the directory we need to look at</param>
    /// <returns>srting in format string with server respond</returns>
    public string List(string path)
    {
        var (size, name) = ListProsess(path);

        return size != -1 ? $"{size} {name}" : "-1";
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
        else
        {
            return (-1, new byte[0]);
        }
    }


    /// <summary>
    /// Sends response for clients reqest and interrupts connection
    /// </summary>
    public async Task ServerMethodAsync()
    {
        var listener = new TcpListener(Ip, Port);
        listener.Start();
        List<Task> list = new List<Task>();
        while (true)
        {
            list.Add(Task.Run(async () =>
            {
                using var socket = await listener.AcceptSocketAsync();
                using var networkStream = new NetworkStream(socket);
                using var streamReader = new StreamReader(networkStream);
                var data = await streamReader.ReadLineAsync();
                var strings = data.Split(' ');
                var requestPath = strings[1];
                switch (int.Parse(strings[0]))
                {
                    case 1: // List request Case
                        {
                            using var streamWriter = new StreamWriter(networkStream);
                            streamWriter.WriteLine(List(DataPath + requestPath));
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
            }));
            foreach (var task in list) {
                await task;
            }
        }
    }
}
