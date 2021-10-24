using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {

            var server = new Server();
            var client = new Client();
            var threads = new Thread[2];
            try
            {
                threads[0] = new Thread(
                    () => server.ServerMethod()
                    );
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                threads[0].Start();
                threads[1] = new Thread(() => client.ClientMethod());
                threads[1].Start();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            /*
            foreach (var thread in threads)
            {
                thread.Join();
            }
            */

            //taskA.Wait();
            //taskB.Wait();
            //string path = "1 ./Tests/Files/testfile.txt";
            //var str = server.List(path);
            //var (res, bytes) =  server.Get(path);
            //Console.WriteLine(res);
            //Console.WriteLine(Encoding.UTF8.GetString(bytes));
        }
    }
}
