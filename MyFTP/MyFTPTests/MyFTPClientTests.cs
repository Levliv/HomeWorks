using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
using MyFTPClient;
using System.Collections;
using System.Linq;
using System.Text;
using System;

namespace MyFTP;

public class MyFTPTests
{
    /// <summary>
    /// Starting up the server to test Client's Get and List methods
    /// </summary>
    [SetUp]
    public void ServerStart()
    {
        IPAddress ip;
        IPAddress.TryParse("127.0.0.1", out ip);
        var server = new Server(ip, 8000);
        var task1 = Task.Run(() => server.ServerMethodAsync());
    }

    /// <summary>
    /// Testing Client's set up ability
    /// </summary>
    [Test]
    public void TestClientSetUp()
    {
        var client = new Client("127.0.0.1", 8000);
        Assert.AreEqual(8000, client.Port);
        Assert.AreEqual("127.0.0.1", client.IpString);
    }

    /// <summary>
    /// Tesing List Request
    /// </summary>
    [Test]
    public void TestClientRequestList()
    {
        var client = new Client("127.0.0.1", 8000);
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
    /// Tesing Get Request
    /// </summary>
    [Test]
    public void TestClientRequestGet()
    {
        
        var client = new Client("127.0.0.1", 8000);
        var result = client.Get("./Tests/Files/TestFile.txt");
        Assert.AreEqual("Abracanabra\r\n2nd line", (Encoding.UTF8.GetString(result.Data)));
    }
}