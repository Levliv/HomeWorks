// <copyright file="ServerEngine.cs" company="IDK">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyFTP;

/// <summary>
/// Base Server class.
/// </summary>
public class ServerEngine
{
    private readonly TcpListener listener;
    private Queue<Task> clientsTaskQueue = new ();

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
    /// Stops the server, all requests recieved before cancellation will be processed.
    /// </summary>
    public CancellationTokenSource Cts { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerEngine"/> class.
    /// Constructor for Server.
    /// </summary>
    /// <param name="dirBackPath">Path to the base directory where the search is supposed to begin.</param>
    public ServerEngine(IPAddress ip, int port, string dirBackPath = "../../../.")
    {
        DataPath = dirBackPath;
        Port = port;
        Ip = ip;
        listener = new TcpListener(Ip, Port);
    }

    /// <summary>
    /// Start serving clients.
    /// </summary>
    public async Task Run()
    {
        listener.Start();
        while (!Cts.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync(Cts.Token);
            var clientTask = Task.Run(() => ServerMethod(socket));
            clientsTaskQueue.Append(clientTask);
        }

        Task.WaitAll(clientsTaskQueue.ToArray());
        listener.Stop();
    }

    /// <summary>
    /// Creates server response.
    /// </summary>
    /// <param name="path">path to the directory we need to look at.</param>
    /// <returns>srting in format string with server response.</returns>
    public async Task<string> ListAsync(string path)
    {
        var (size, name) = await ListProсessAsync(path);
        return size != -1 ? $"{size} {name}" : "-1";
    }

    /// <summary>
    /// Sends a datagram as a response to Get request.
    /// </summary>
    /// <param name="streamWriter">File stream to push results in.</param>
    /// <param name="path">File path.</param>
    private async Task GetServerAsync(StreamWriter streamWriter, string path)
    {
        if (File.Exists(path))
        {
            var size = new FileInfo(path).Length;
            await streamWriter.WriteAsync($"{size} ");
            await streamWriter.FlushAsync();
            using var fileStream = File.Open(path, FileMode.Open);
            await fileStream.CopyToAsync(streamWriter.BaseStream);
        }
        else
        {
            await streamWriter.WriteLineAsync("-1 ");
        }

        await streamWriter.FlushAsync();
    }

    /// <summary>
    /// Server's method for seraching in order to create list of files and dirs in the directory.
    /// </summary>
    private async Task<(int size, string name)> ListProсessAsync(string path)
    {
        var directory = new DirectoryInfo(path);
        if (directory.Exists)
        {
            var (numberOfFiles, strFiles) = ProсessFiles(directory.GetFiles());
            var (numberOfDirectories, strDirs) = ProсessDirectories(directory.GetDirectories());
            return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs);
        }

        return (-1, string.Empty);
    }

    private async Task ServerMethod(Socket socket)
    {
        using var networkStream = new NetworkStream(socket);
        using var streamReader = new StreamReader(networkStream);
        using var streamWriter = new StreamWriter(networkStream);
        var data = await streamReader.ReadLineAsync();
        var strings = data.Split(' ');
        var requestPath = strings[1];
        switch (int.Parse(strings[0]))
        {
            case 1: // List request Case
                {
                    await streamWriter.WriteLineAsync(await ListAsync(DataPath + requestPath));
                    await streamWriter.FlushAsync();
                    return;
                }

            case 2: // Get request Case
                {
                    await GetServerAsync(streamWriter, DataPath + requestPath);
                    return;
                }
        }
    }

    private (int, string) ProсessDirectories(DirectoryInfo[] directories)
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

    private (int, string) ProсessFiles(FileInfo[] files)
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
}