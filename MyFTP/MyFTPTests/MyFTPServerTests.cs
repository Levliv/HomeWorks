using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
namespace MyFTP;

class MyFTPServerTests
{
    [SetUp]
    public void ServerSetUp()
    {
        IPAddress ip;
        IPAddress.TryParse("127.0.0.1", out ip);
        var server = new Server(ip, 8000);
        var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
    }

    [Test]
    public void TestGet()
    {

    }

    [Test]
    public void TestList()
    {

    }
}
