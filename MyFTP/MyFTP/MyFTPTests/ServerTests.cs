using NUnit.Framework;
using System.Net;
using System.Text;
namespace MyFTP;

/// <summary>
/// Tests for server's part.
/// </summary>
internal class MyFTPServerTests
{
    private ServerEngine server;

    /// <summary>
    /// Seting up the server
    /// </summary>
    [OneTimeSetUp]
    public void ServerSetUp()
    {
        IPAddress ip;
        IPAddress.TryParse("127.0.0.1", out ip);
        server = new ServerEngine(ip, 8000);
        server.Run();
    }

    /// <summary>
    /// Testing Server's get method
    /// </summary>
    [Test]
    public void TestServerGet()
    {
        string path = "../../../.." + "/Tests/Files/TestFile.txt";
        var (size, data) = server.Get(path);
        Assert.AreEqual(21, size);
        Assert.AreEqual("Abracanabra\r\n2nd line", Encoding.UTF8.GetString(data));
    }

    /// <summary>
    /// Testing Server's List method
    /// </summary>
    [Test]
    public void TestServerList()
    {
        string path = "../../../.." + "/Tests/Files";
        var responseString = server.List(path).Result;
        Assert.AreEqual("2 ./Tests/Files/TestFile.txt false ./Tests/Files/TestDir true", responseString);
    }
}