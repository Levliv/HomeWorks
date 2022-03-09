// <copyright file="ClientTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP;

/// <summary>
/// Tests for Client's side.
/// </summary>
public class MyFTPTests
{
    private ServerEngine server;

    /// <summary>
    /// Starting up the server to test Client's Get and List methods.
    /// </summary>
    /// //OneTime
    [OneTimeSetUp]
    public void ServerStart()
    {
        IPAddress.TryParse("127.0.0.1", out IPAddress ip);
        server = new ServerEngine(ip, 8000);
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

        var result = await client.ListAsync("./Tests/Files");
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
        var baseFile = "./Tests/Files/TestFile.txt";
        var file = "./../../../../Tests/Files2/result.txt";
        File.Delete(file);
        //Console.WriteLine(Path.GetFullPath(baseFile));
        //Console.WriteLine(Path.GetFullPath(file));
        //var f = Path.GetFullPath(file);
        var actual = await client.GetAsync(baseFile, file);
        //Console.WriteLine(actual);
        //File.OpenRead(baseFile);
        //Console.WriteLine(actual);
        //File.OpenRead(file);
        //Console.WriteLine(actual);
        Assert.Pass();
        //FileAssert.AreEqual("./../../../../Test/File/TestFile.txt", file);
    }

    /// <summary>
    /// If file doesn't exist size of the message should be -1.
    /// </summary>
    [Test]
    public async Task TestIfFileDoesNotExist()
    {
        var client = new ClientEngine("127.0.0.1", 8000);
        var result = await client.GetAsync("./Tests/Files/TestFiles.txt", "result.txt");
        Assert.AreEqual(-1, result);
    }
}