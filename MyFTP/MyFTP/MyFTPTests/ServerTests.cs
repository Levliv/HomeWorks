// <copyright file="ServerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace MyFTP;

/// <summary>
/// Tests for server's part.
/// </summary>
internal class MyFTPServerTests
{
    private ServerEngine server;

    /// <summary>
    /// Seting up the server.
    /// </summary>
    [OneTimeSetUp]
    public void ServerSetUp()
    {
        IPAddress.TryParse("127.0.0.1", out IPAddress? ip);
        server = new ServerEngine(ip, 8000);
        server.Run();
    }

    /// <summary>
    /// Testing Server's List method.
    /// </summary>
    [TestCase(ExpectedResult = "2 ./Tests/Files/TestFile.txt false ./Tests/Files/TestDir true")]
    public async Task<string> TestServerList()
    {
        string path = "../../../.." + "/Tests/Files";
        var responseString = await server.ListAsync(path);
        return responseString;
    }
}