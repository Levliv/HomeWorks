using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
using MyFTPClient;
using System.Collections;
using System.Linq;
using System.Text;

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
        client.ClientRequest("1 ./Tests/Files");
        Assert.AreEqual(client.FtpRequestType, RequestType.List);
        var fileNames = from item in client.ResultsOfListResponse select item.Name;
        var ifFilesDirs = from item in client.ResultsOfListResponse select item.IsDir;
        IEnumerable expectedFileNames = new[] { "./Tests/Files/TestFile.txt", "./Tests/Files/TestDir" };
        IEnumerable expectedIsFilesdirs = new[] {false, true };
        Assert.AreEqual(expectedFileNames, fileNames);
        Assert.AreEqual(expectedIsFilesdirs, ifFilesDirs);
    }

    /// <summary>
    /// Tesing Get Request
    /// </summary>
    [Test]
    public void TestClientRequestGet()
    {
        var client = new Client("127.0.0.1", 8000);
        client.ClientRequest("2 ./Tests/Files/TestFile.txt");
        Assert.AreEqual("Abracanabra\r\n2nd line", (Encoding.UTF8.GetString(client.GetResponse.Data)));
    }
}