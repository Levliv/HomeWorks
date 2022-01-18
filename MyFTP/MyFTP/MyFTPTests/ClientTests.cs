﻿using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace MyFTP;

/// <summary>
/// Tests for Client's side.
/// </summary>
public class MyFTPTests
{
    /// <summary>
    /// Starting up the server to test Client's Get and List methods.
    /// </summary>
    [OneTimeSetUp]
    public void ServerStart()
    {
        IPAddress.TryParse("127.0.0.1", out IPAddress ip);
        var server = new ServerEngine(ip, 8000);
        server.Run();
    }

    /// <summary>
    /// Testing Client's set up ability.
    /// </summary>
    [Test]
    public void TestClientSetUp()
    {
        var client = new ClientEngine("127.0.0.1", 8000);
        Assert.AreEqual(8000, client.Port);
        Assert.AreEqual("127.0.0.1", client.IpString);
    }

    /// <summary>
    /// Testing List Request.
    /// </summary>
    [Test]
    public async Task TestClientRequestList()
    {
        var client = new ClientEngine("127.0.0.1", 8000);
        IEnumerable expectedFileNames = new[] { "./Tests/Files/TestFile.txt False", "./Tests/Files/TestDir True" };
        var expectedString = new StringBuilder();
        foreach (var fileName in expectedFileNames)
        {
            expectedString.Append(fileName);
            expectedString.Append(' ');
        }
        var result = client.List("./Tests/Files");
        var resultString = new StringBuilder();
        foreach (var file in result)
        {
            resultString.Append(file.Name);
            resultString.Append(' ');
            resultString.Append(file.IsDir);
            resultString.Append(' ');
        }
        Assert.IsTrue(expectedString.Equals(resultString));
    }

    /// <summary>
    /// Testing Get Request.
    /// </summary>
    [Test]
    public async Task TestClientRequestGet()
    {
        var client = new ClientEngine("127.0.0.1", 8000);
        var result = await client.Get("./Tests/Files/TestFile.txt");
        Assert.AreEqual("Abracanabra\r\n2nd line", Encoding.UTF8.GetString(result.Data));
    }
}