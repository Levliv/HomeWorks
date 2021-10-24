using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyFTP
{
    class Server: IServer
    {
        private (int, string) ProsessFiles(FileInfo[] files)
        {
            var stringBuilder = new StringBuilder();
            int currentDirectoryLenght = Directory.GetCurrentDirectory().Length;
            foreach (var file in files)
                stringBuilder.Append("." + file.ToString().Substring(currentDirectoryLenght).Replace('\\', '/') + " false");
            var ResultString = stringBuilder.ToString();
            return (files.Length, ResultString);
        }

        private (int, string) ProsessDirectories(DirectoryInfo[] directories)
        {
            var stringBuilder = new StringBuilder();
            int currentDirectoryLenght = Directory.GetCurrentDirectory().Length;
            foreach (var directory in directories)
                stringBuilder.Append("." + directory.ToString().Substring(currentDirectoryLenght).Replace('\\', '/') + " true");
            var ResultString = stringBuilder.ToString();
            return (directories.Length, ResultString);
        }

        private (int size, string name, bool isDir) ListProsess(string path)
        {
            var stringShorted = path.Substring(2);
            var pathWithChangedSlashes = stringShorted.Replace('/', '\\');
            var di = new DirectoryInfo(pathWithChangedSlashes);
            if (di.Exists)
            {
                var (numberOfFiles, strFiles) = ProsessFiles(di.GetFiles());
                var (numberOfDirectories, strDirs) = ProsessDirectories(di.GetDirectories());
                return (numberOfFiles + numberOfDirectories, strFiles + " " + strDirs, di.Exists);
            }
            return (-1, "", di.Exists);
        }

        /// <summary>
        /// Creates server respond
        /// </summary>
        /// <param name="path">path to the directory we need to look at</param>
        /// <returns>srting in format string with server respond</returns>
        public string List(string path)
        {
            var (size, name, isDir) = ListProsess(path);
            if (size != -1)
            {
                return size.ToString() + " " + name + " " + isDir.ToString();
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
        public (long size, byte[] content) Get(string path)
        {
            var stringShorted = path.Substring(2);
            var pathWithChangedSlashes = stringShorted.Replace('/', '\\');
            if (File.Exists(pathWithChangedSlashes))
            {
                var dataBytes = File.ReadAllBytes(pathWithChangedSlashes);
                return (dataBytes.Length, dataBytes);
            } else
            {
                return (-1, new byte[0]);
            }
        }

        public static void ServerMethod()
        {
            Console.WriteLine("Starting");
            const int port = 8888;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Listening on port: {port}");
            using (var socket = listener.AcceptSocket())
            {
                var stream = new NetworkStream(socket);
                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadLine();
                Console.WriteLine($"Recived: {data}");

                Console.WriteLine($"Sending \"Hi!\"");
                var streamWriter = new StreamWriter(stream);
                streamWriter.WriteLine("Hi!");
                streamWriter.Flush();
            }
            listener.Stop();
            Console.WriteLine("Finishing");
        }
        public static void ClientMethod()
        {
            const int port = 8888;
            using (var client = new TcpClient("localhost", port))
            {
                Console.WriteLine($"Sending to port: {port}");
                var stream = client.GetStream();
                var streamWrirter = new StreamWriter(stream);
                streamWrirter.WriteLine("Hello World!");
                streamWrirter.Flush();

                Console.WriteLine($"Reciving on port: {port}");
                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadLine();
                Console.WriteLine($"Recived data: {data}");
            }
        }
    }
}
