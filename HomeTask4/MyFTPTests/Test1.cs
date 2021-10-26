using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.IO;

namespace MyFTP
{
    public class Tests
    {
        [Test]
        public void TestServerList()
        {
            var server = new Server();
            var client = new Client("1 ./Tests/Files");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            //Console.WriteLine("Test WriteLine");
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(client.ReceivedData));
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "2 ./Tests/Files/TestFile.txt false ./Tests/Files/Testdir true ");
            //Console.WriteLine(Directory.GetCurrentDirectory());
        }

        [Test]
        public void TestServerGet()
        {
            var server = new Server();
            var client = new Client("2 ./Tests/Files/TestFile.txt");
            var task1 = Task.Run(() => server.ServerMethodAsync().Wait());
            client.ClientMethod();
            Console.WriteLine("Test WriteLine");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(client.ReceivedData));
            //Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "2 ./Tests/Files/TestFile.txt false ./Tests/Files/Testdir true ");
        }
    }
}