using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFTP
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var server = new Server();
            var client = new Client("1 ./Tests/Files");
            var task1 = Task.Run(() => server.ServerMethodAsync());
            var task2 = Task.Run(() => client.ClientMethod());
            task1.Wait();
            task2.Wait();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(client.ReceivedData), "2 ./Tests/Files/testfile.txt false ./Tests/Files/testdir true");
        }
    }
}