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
    /// Gets cancellation token that can be used to stop the server.
    /// </summary>
    public CancellationTokenSource Cts { get; private set; } = new ();

    /// <summary>
    /// Gets or sets port for the server.
    /// </summary>
    private int Port;

    /// <summary>
    /// Gets ip of the server.
    /// </summary>
    private IPAddress Ip;

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

    /// <summary>
    /// Stops the server with Cancellation token, all requests recivied before cancelletion will be processed.
    /// </summary>
    public void Stop() => Cts.Cancel();

    /// <summary>
    /// Creates server response.
    /// </summary>
    /// <param name="path"> Path to the directory we need to look at. </param>
    /// <returns> -1 if files weren't found, else string with paths and meta info whether it is a directory </returns>
    public string List(string path)
    {
        try
        {
            var currentDirectory = new DirectoryInfo(path);
            var files = currentDirectory.GetFiles();
            var directories = currentDirectory.GetDirectories();

            return $"{files.Length + directories.Length} {string.Join(string.Empty, files.Select(x => $"{x.Name} False "))}" +
                $"{string.Join(string.Empty, directories.Select(x => $"{x.Name} True "))}";

        }
        catch (DirectoryNotFoundException)
        {
            return "-1 ";
        }
        catch (FileNotFoundException)
        {
            return "-1 ";
        }

    }

    /// <summary>
    /// Sends a datagram as a response to Get request.
    /// </summary>
    /// <param name="streamWriter"> File stream to push result in. </param>
    /// <param name="path"> File path. </param>
    public async Task GetServerAsync(StreamWriter streamWriter, string path)
    {
        try
        {
            var size = new FileInfo(path).Length;
            await streamWriter.WriteAsync($"{size} ");
            await using var fileStream = new FileStream(path, FileMode.Open);
            await fileStream.CopyToAsync(streamWriter.BaseStream);
            await streamWriter.WriteLineAsync();
        }
        catch (FileNotFoundException)
        {
            await streamWriter.WriteLineAsync("-1 ");
        }
        catch (IOException)
        {
            await streamWriter.WriteLineAsync("-1 ");
        }
    }

    private async Task ServerMethod(Socket socket)
    {
        await using var networkStream = new NetworkStream(socket);
        using var streamReader = new StreamReader(networkStream);
        await using var streamWriter = new StreamWriter(networkStream) { AutoFlush = true };
        var data = await streamReader.ReadLineAsync();
        ArgumentNullException.ThrowIfNull(data, "Client shouldn't send empty requests");
        var stringsCommandPath = data.Split(' ');
        var command = int.Parse(stringsCommandPath[0]);
        var path = stringsCommandPath[1];
        switch (command)
        {
            case 1: // List request Case
                {
                    await streamWriter.WriteLineAsync(List(path));
                    streamWriter.Close();
                    break;
                }

            case 2: // Get request Case
                {
                    await GetServerAsync(streamWriter, path);
                    streamWriter.Close();
                    break;
                }

            default:
                await streamWriter.WriteLineAsync($"Command {command} was not found");
                break;
        }
    }
}