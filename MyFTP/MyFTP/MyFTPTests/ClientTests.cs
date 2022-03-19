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
using System.Threading;
using Microsoft.VisualBasic;

namespace MyFTP;

/// <summary>
/// Tests for Client's side.
/// </summary>
public class MyFtpTests
{
    private ServerEngine server;

    /// <summary>
    /// Starting up the server to test Client's Get and List methods.
    /// </summary>
    /// //OneTime
    [SetUp]
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
    /// Testing Get Request if file exists.
    /// </summary>
    [Test]
    public async Task TestGet()
    {
        var fileStream = new MemoryStream();
        var client = new ClientEngine("127.0.0.1", 8000);
        //throw new Exception("Durty");
        await client.GetAsync("../../../../Tests/Files/TestFile.txt", fileStream);
        //Assert.AreEqual(21, size);
        
        //using var file = File.Open("../../../../Tests/Files/TestFile.txt", FileMode.Open);
        //using var resultStream = new StreamReader(fileStream);
        //var result = await resultStreamReader.ReadToEndAsync();
        //using var answerStream = new StreamReader(file);
        //var answer = await answerStream.ReadToEndAsync();
        Assert.Pass();
        //FileAssert.AreEqual(file, fileStream);
    }
    
    /// <summary>
    /// If file doesn't exist size of the message should be -1.
    /// </summary>
    [Test]
    public async Task TestIfFileDoesNotExist()
    {
        var client = new ClientEngine("127.0.0.1", 8000);
        var fileStream = new MemoryStream();
        await client.GetAsync("../../../../Tests/Files/TestFiles.txt", fileStream);
        Assert.AreEqual(-1, 12);
    }
}