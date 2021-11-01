using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.IO;

namespace MyFTP
{
    public class Tests
    {
        private string dataPath = "../../../Tests";

        [Test]
        public void TestServerList()
        {
            var server = new Server();
            var client = new Client("1 .../../../../MyFTP.Tests/Data/TestFile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "2 ./Tests/Files/TestFile.txt false ./Tests/Files/Testdir true ");
        }

        [Test]
        public void TestServerGet()
        {
            var server = new Server();
            var client = new Client("2 ./Tests/Files/TestFile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "21\nabracadabra\r\n2nd line");
        }

        [Test]
        public void TestServerForEmptyGet()
        {
            var server = new Server();
            var client = new Client("2 ./Tests/Files/TestBile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "-1\n");
        }
        [Test]
        public void TestForWrongRequest()
        {
            var server = new Server();
            var client = new Client("3 ./Tests/Files/TestBile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual(client.ReceivedData, "");
        }
    }
}