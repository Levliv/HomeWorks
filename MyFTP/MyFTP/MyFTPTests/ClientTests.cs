// <copyright file="ClientTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Threading;
using Microsoft.VisualBasic;

namespace MyFTP;

/// <summary>
/// Tests for Client's side.
/// </summary>
public class MyFtpTests
{
    private const string ip = "127.0.0.1";
    private ServerEngine server = new (IPAddress.Parse(ip), 8000);
    private ClientEngine client = new (ip, 8000);
    /// <summary>
    /// Starting up the server to test Client's Get and List methods.
    /// </summary>
    /// //OneTime
    [SetUp]
    public void ServerStart()
    {
        server.Run();
    }

    /// <summary>
    /// Testing Client's set up ability.
    /// </summary>
    [Test]
    public void TestClientSetUp()
    {
        Assert.AreEqual(8000, client.Port);
        Assert.AreEqual("127.0.0.1", client.IpString);
    }

    [Test]
    public async Task TestClientGetTextFile()
    {
        var fileDirectory = "../../../../Results";
        Directory.CreateDirectory(fileDirectory);
        var source = "../../../../Tests/Files/TestFile.txt";
        var target = "../../../../Results/ResultFile.txt";
        await client.GetAsync(source, target);
        FileAssert.AreEqual(source, target);
        Directory.Delete(fileDirectory, true);
    }

    /// <summary>
    /// Testing List Request.
    /// </summary>
    [Test]
    public async Task TestClientRequestList()
    {
        var sourceDir = "../../../../Tests/Files/";
        var expectedFileNames = "TestFile.txt False TestDir True ";
        var expectedString = new StringBuilder();
        foreach (var fileName in expectedFileNames)
        {
            expectedString.Append(fileName);
            expectedString.Append(' ');
        }

        var (size, result) = await client.ListAsync(sourceDir);
        Assert.AreEqual(2, size);
        var resultString = new StringBuilder();
        foreach (var file in result)
        {
            resultString.Append(string.Join(" ", file.Item1,  file.Item2));
            resultString.Append(' ');
        }

        Assert.AreEqual(expectedFileNames, resultString.ToString());
    }
}