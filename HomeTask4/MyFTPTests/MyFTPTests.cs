using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;

namespace MyFTP
{
    public class MyFTPTests
    {
        [TestCase("1 ./Tests/Files", TestName = "TestServerList", ExpectedResult = "2 ./Tests/Files/TestFile.txt false ./Tests/Files/Testdir true")]
        [TestCase("2 ./Tests/Files/TestFile.txt", TestName = "TestServerGet", ExpectedResult = "23\nAbracadabra\r\n2nd line\r\n")]
        [TestCase("2 ./Tests/Files/TestBile.txt", TestName = "TestServerForEmptyGet", ExpectedResult = "-1\n")]
        [TestCase("3 ./Tests/Files/TestBile.txt", TestName = "TestForWrongRequest", ExpectedResult = "")]
        public string TestServer(string request)
        {
            IPAddress ip;
            IPAddress.TryParse("127.0.0.1", out ip);
            var server = new Server(ip, 8888);
            var client = new Client(request, ip, 8888);
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            return System.Text.Encoding.UTF8.GetString(client.FileData);
        }
    }
}