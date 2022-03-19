// <copyright file="ServerEngine.cs" company="PlaceholderCompany">
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
    private readonly Queue<Task> clientsTaskQueue = new ();

    /// <summary>
    /// Gets port for the server.
    /// </summary>
    private int Port { get; set; }

    /// <summary>
    /// Gets ip of the server.
    /// </summary>
    private IPAddress Ip { get; set; }

    /// <summary>
    /// Gets stops the server, all requests received before cancellation will be processed.
    /// </summary>
    public CancellationTokenSource Cts { get; private set; } = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerEngine"/> class.
    /// Constructor for Server.
    /// </summary>
    public ServerEngine(IPAddress ip, int port)
    {
        Port = port;
        Ip = ip;
        listener = new TcpListener(Ip, Port);
    }

    /// <summary>
    /// Start serving clients.
    /// </summary>
    public async Task Run()
    {
        Console.WriteLine("Started");
        listener.Start();
        while (!Cts.Token.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync(Cts.Token);
            var clientTask = Task.Run(() => ServerMethod(socket));
            clientsTaskQueue.Append(clientTask);
        }

        Task.WaitAll(clientsTaskQueue.ToArray());
        listener.Stop();
    }

    public void Stop() => Cts.Cancel();

    /// <summary>
    /// Creates server response.
    /// </summary>
    /// <param name="path"> Path to the directory we need to look at. </param>
    /// <returns> -1 if files weren't found, else string with paths and meta info whether it is a directory </returns>
    public async Task<string> ListAsync(string path)
    {
        var (size, name) = await ListProсessAsync(path);
        return size != -1 ? $"{size} {name}" : "-1";
    }

    /// <summary>
    /// Sends a datagram as a response to Get request.
    /// </summary>
    /// <param name="streamWriter"> File stream to push results in. </param>
    /// <param name="path"> File path. </param>
    private async Task GetServerAsync(StreamWriter streamWriter, string path)
    {
        if (File.Exists(path))
        {
            var size = new FileInfo(path).Length;
            await streamWriter.WriteAsync($"{size} ");
            await using var fileStream = new FileStream(path, FileMode.Open);
            await fileStream.CopyToAsync(streamWriter.BaseStream);
            await streamWriter.WriteLineAsync();
        }
        else
        {
            await streamWriter.WriteLineAsync("-1 ");
        }
    }

    /// <summary>
    /// Server's method for searching in order to create list of files and dirs in the directory.
    /// </summary>
    private async Task<(int size, string name)> ListProсessAsync(string path)
    {
        var directory = new DirectoryInfo(path);
        if (directory.Exists)
        {
            var t1 = Task.Run(() => ProсessFiles(directory.GetFiles()));
            var t2 = Task.Run(() => ProсessDirectories(directory.GetDirectories()));
            var (numberOfFiles, strFiles) = await t1;
            var (numberOfDirectories, strDirs) = await t2;
            return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs);
        }

        return (-1, string.Empty);
    }

    private async Task ServerMethod(Socket socket)
    {
        await using var networkStream = new NetworkStream(socket);
        using var streamReader = new StreamReader(networkStream);
        await using var streamWriter = new StreamWriter(networkStream) { AutoFlush = true };
        var data = await streamReader.ReadLineAsync();
        var strings = data.Split(' ');
        var requestPath = strings[1];
        switch (int.Parse(strings[0]))
        {
            case 1: // List request Case
                {
                    await streamWriter.WriteLineAsync(await ListAsync(requestPath));
                    return;
                }

            case 2: // Get request Case
                {
                    await GetServerAsync(streamWriter, requestPath);
                    return;
                }
        }
    }

    private (int, string) ProсessDirectories(DirectoryInfo[] directories)
    {
        var stringBuilder = new StringBuilder();
        foreach (var directory in directories)
        {
            stringBuilder.Append(directory.ToString().Replace('\\', '/') + " true");
        }

        var resultString = stringBuilder.ToString();
        return (directories.Length, resultString);
    }

    private (int, string) ProсessFiles(FileInfo[] files)
    {
        var stringBuilder = new StringBuilder();
        foreach (var file in files)
        {
            stringBuilder.Append(file.ToString().Replace('\\', '/') + " false");
        }

        var resultString = stringBuilder.ToString();
        return (files.Length, resultString);
    }
}