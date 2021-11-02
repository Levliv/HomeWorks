using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP
{
    /// <summary>
    /// Server class to process requests
    /// </summary>
    public class Server
    {
        private (int, string) ProsessFiles(FileInfo[] files)
        {
            var stringBuilder = new StringBuilder();
            var dir = Path.GetFullPath("../../../../..");
            Console.WriteLine("Dir");
            Console.WriteLine(dir);
            foreach (var file in files)
            {
                stringBuilder.Append("." + file.ToString().Replace(dir, "").Replace('\\', '/') + " false");
            }
            var ResultString = stringBuilder.ToString();
            return (files.Length, ResultString);
        }

        private (int, string) ProsessDirectories(DirectoryInfo[] directories)
        {
            var stringBuilder = new StringBuilder();
            Console.WriteLine("Good boy");
            Console.WriteLine(Directory.GetCurrentDirectory());
            var dir = Path.GetFullPath("../../../../..");
            foreach (var directory in directories)
            {
                stringBuilder.Append("." + directory.ToString().Replace(dir, "").Replace('\\', '/') + " true");
                Console.WriteLine(directory.ToString());
            }
            var ResultString = stringBuilder.ToString();
            return (directories.Length, ResultString);
        }

        private (int size, string name) ListProsess(string path)
        {
            Console.WriteLine("List Prosess: " + path);
            var di = new DirectoryInfo(path);
            Console.WriteLine(di.Exists);
            if (di.Exists)
            {
                Console.WriteLine("Inside");
                Console.WriteLine(di.GetFiles().Length);
                var (numberOfFiles, strFiles) = ProsessFiles(di.GetFiles());
                Console.WriteLine(numberOfFiles);
                var (numberOfDirectories, strDirs) = ProsessDirectories(di.GetDirectories());
                Console.WriteLine(numberOfDirectories);
                return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs);
            }
            return (-1, "");
        }

        /// <summary>
        /// Creates server respond
        /// </summary>
        /// <param name="path">path to the directory we need to look at</param>
        /// <returns>srting in format string with server respond</returns>
        private string List(string path)
        {
            var (size, name) = ListProsess(path);
            if (size != -1)
            {
                return size.ToString() + " " + name + " ";
            }
            else
            {
                return "-1";
            }
        }

        /// <summary>
        /// Returns datagram in respond of Get request
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Size of file, massive of bytes(file)</returns>
        private (long size, byte[] content) Get(string path)
        {
            if (File.Exists(path))
            {
                var dataBytes = File.ReadAllBytes(path);
                return (dataBytes.Length, dataBytes);
            } else
            {
                return (-1, new byte[0]);
            }
        }

        /// <summary>
        /// Sends response for clients reqest and interrupts connection
        /// </summary>
        public async Task ServerMethodAsync()
        {
            const int port = 8888;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (true)
            {
                using var socket = await listener.AcceptSocketAsync();
                await Task.Run(async () =>
                {
                    var stream = new NetworkStream(socket);
                    var streamReader = new StreamReader(stream);
                    var data = await streamReader.ReadLineAsync();
                    var strings = data.Split(' ');
                    var streamBinaryWriter = new BinaryWriter(stream);
                    Console.WriteLine("Here Server");
                    Console.WriteLine("Got:" + strings[0]);
                    switch (int.Parse(strings[0]))
                    {
                        case 1:
                            {
                                Console.WriteLine("Case1");
                                Console.WriteLine("Got:" + strings[1]);
                                streamBinaryWriter.Write(Encoding.UTF8.GetBytes(List(strings[1]))); //.Replace('/', '\\')
                                Console.WriteLine("AfterStreamWriter");
                                Console.WriteLine("Got:" + strings[1]);
                                streamBinaryWriter.Flush();
                                break;
                            }
                        case 2:
                            {
                                var (size, bytes) = Get(strings[1].Substring(2)); //.Replace('/', '\\')
                                
                                var sizeInBytes = Encoding.UTF8.GetBytes(size.ToString());
                                streamBinaryWriter.Write(sizeInBytes);
                                streamBinaryWriter.Write(Encoding.UTF8.GetBytes("\n"));
                                streamBinaryWriter.Write(bytes);
                                streamBinaryWriter.Flush();
                                break;
                            }
                    }
                });
            }
        }
    }
}
