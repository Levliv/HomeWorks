using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.IO;

namespace MyFTP
{
    public class Tests
    {
        private string dataPath = "../../../../..";

        [Test]
        public void TestServerList()
        {
            var server = new Server();
            Console.WriteLine("Ok-1");
            var client = new Client($"1 .{dataPath}/Tests/Files");

            Console.WriteLine("Ok-2");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual("2 ./Tests/Files/TestFile.txt false ./Tests/Files/Testdir true ", System.Text.Encoding.UTF8.GetString(client.ReceivedData));
        }

        [Test]
        public void TestServerGet()
        {
            var streamWriter = new StreamWriter("Tests\\Files\\TestFile.txt");
            streamWriter.Write("abracadabra\r\n2nd line");
            streamWriter.Close();
            var server = new Server();
            var client = new Client("2 ./Tests/Files/TestFile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual("21\nabracadabra\r\n2nd line", System.Text.Encoding.UTF8.GetString(client.ReceivedData));
        }

        [Test]
        public void TestServerForEmptyGet()
        {
            var server = new Server();
            var client = new Client("2 ./Tests/Files/TestBile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual("-1\n", System.Text.Encoding.UTF8.GetString(client.ReceivedData));
        }
        [Test]
        public void TestForWrongRequest()
        {
            var server = new Server();
            var client = new Client("3 ./Tests/Files/TestBile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Assert.AreEqual("", client.ReceivedData);
        }
    }
}