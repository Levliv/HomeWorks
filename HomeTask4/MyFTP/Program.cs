using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {


            //var taskA = new Task(() => Server.ServerMethod());
            //var taskB = new Task(() => Server.ClientMethod());
            //taskA.Start();
            //taskB.Start();
            //taskA.Wait();
            //taskB.Wait();
            string path = "1 ./Tests/Files";
            var server = new Server();
            var str = server.List(path);
            Console.WriteLine(str);
        }
    }
}
