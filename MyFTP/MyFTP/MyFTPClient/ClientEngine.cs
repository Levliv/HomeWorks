// <copyright file="ClientEngine.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Text;

namespace MyFTP;

public class ClientEngine
{
    /// <summary>
    /// Tcp Client string information about current connection.
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
    /// Ip to listen.
    /// </summary>
    public string IpString { get; private set; }

    /// <summary>
    /// Port to listen.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// List request method, getting information about the files and directories found by the provided path.
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
        await streamWriter.WriteLineAsync($"1 {path}");
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
    /// Getting the file data, storing in the current file, by provided path.
    /// </summary>
    /// <param name="path">provided relative path.</param>
    /// <returns> Base struct GetResponseStruct.</returns>
    public async Task<int> GetAsync(string path, string pathOnClient)
    {
        Console.WriteLine($"Path: {Path.GetFullPath(path)}");
        if (!tcpClient.Connected)
        {
            await tcpClient.ConnectAsync(IpString, Port);
        }

        using var networkStream = tcpClient.GetStream();
        using var streamWriter = new StreamWriter(networkStream);
        await streamWriter.WriteLineAsync($"2 {path} ");
        await streamWriter.FlushAsync();
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

        await using var filestream = File.Create(pathOnClient);
        await networkStream.CopyToAsync(filestream);
        /*
        using var t = new FileStream(pathOnClient, FileMode.OpenOrCreate);
        await networkStream.CopyToAsync(t);
        */
        var messageLength = Convert.ToInt32(size.ToString());
        Console.WriteLine($"Ok, {messageLength}");
        return messageLength;
    }
}
