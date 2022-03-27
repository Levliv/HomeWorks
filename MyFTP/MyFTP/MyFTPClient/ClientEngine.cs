// <copyright file="ClientEngine.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Text;

namespace MyFTP;

public class ClientEngine
{
    /// <summary>
    /// Gets stops the Client, all requests sent before cancellation will be processed.
    /// </summary>
    public CancellationTokenSource Cts { get; private set; } = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEngine"/> class.
    /// Constructor for the Client, creating new TCP client and connecting to the server.
    /// </summary>
    public ClientEngine(string ipString, int port)
    {
        IpString = ipString;
        Port = port;
    }

    /// <summary>
    /// Gets ip to listen.
    /// </summary>
    public string IpString { get; private set; }

    /// <summary>
    /// Gets port to listen.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// List request method, getting information about the files and directories found by the provided path.
    /// </summary>
    /// <param name="path">provided path, where to look</param>
    /// <returns>Sequence of data in base ResponseFormat</returns>
    public async Task<(int, List<(string, bool)>)> ListAsync(string path)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(IpString, Port, Cts.Token);
        using var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream) { AutoFlush = true };
        await streamWriter.WriteLineAsync($"1 {path}");
        using var streamReader = new StreamReader(networkStream);
        var message = await streamReader.ReadLineAsync();
        var strings = message.Split();
        var files = new List<(string, bool)>();
        if (strings == null)
        {
            return (-1, null);
        }

        var numberOfObjectsFound = int.Parse(strings[0]);
        for (var i = 1; i < numberOfObjectsFound * 2; i += 2)
        {
            files.Add((strings[i], bool.Parse(strings[i + 1])));
        }

        return (numberOfObjectsFound, files);
    }

    /// <summary>
    /// Getting the file data, storing in the current file, by provided path.
    /// </summary>
    /// <param name="path"> provided relative path. </param>
    /// <param name="destination"> Where file should be stored. </param>
    /// <returns> Size of the received file.</returns>
    public async Task<int> GetAsync(string path, string destination)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(IpString, Port, Cts.Token);
        using var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream) { AutoFlush = true };
        await streamWriter.WriteLineAsync($"2 {path}");
        using var streamReader = new StreamReader(networkStream);
        var size = new StringBuilder();
        int symbol = new ();
        while ((symbol = streamReader.Read()) != ' ')
        {
            if (symbol == '-' && (char)streamReader.Read() == '1')
            {
                return -1;
            }

            size.Append((char)symbol);
        }

        using var destinationStream = File.Create(destination);
        var messageLength = Convert.ToInt32(size.ToString());
        var bufferSize = 1;
        var buffer = new byte[bufferSize];
        for (int i = 0; i < messageLength; i++)
        {
            await networkStream.ReadAsync(buffer, Cts.Token);
            await destinationStream.WriteAsync(buffer, Cts.Token);
        }

        return messageLength;
    }
}
