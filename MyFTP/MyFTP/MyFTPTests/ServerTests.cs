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
internal class MyFtpServerTests
{

    private ServerEngine server = new ServerEngine(IPAddress.Parse("127.0.0.1"), 8000);

    /// <summary>
    /// Setting up the server.
    /// </summary>
    [OneTimeSetUp]
    public void ServerSetUp()
    {
        server.Run();
    }

    /// <summary>
    /// Testing Server's List method.
    /// </summary>
    [TestCase(ExpectedResult = "2 TestFile.txt False TestDir True ")]
    public async Task<string> TestServerList()
    {
        string path = "../../../.." + "/Tests/Files";
        var responseString = server.List(path);
        return responseString;
    }
}