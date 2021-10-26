using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP
{
    public class Server
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

        private (int size, string name) ListProsess(string path)
        {
            var di = new DirectoryInfo(path);
            if (di.Exists)
            {
                var (numberOfFiles, strFiles) = ProsessFiles(di.GetFiles());
                var (numberOfDirectories, strDirs) = ProsessDirectories(di.GetDirectories());
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
            Console.WriteLine($"Listening on port: {port}");
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                await Task.Run(async () =>
                {
                    var stream = new NetworkStream(socket);
                    var streamReader = new StreamReader(stream);
                    var data = await streamReader.ReadLineAsync();
                    var strings = data.Split(' ');
                    var streamBinaryWriter = new BinaryWriter(stream);
                    switch (int.Parse(strings[0]))
                    {
                        case 1:
                            {
                                streamBinaryWriter.Write(Encoding.UTF8.GetBytes(List(strings[1].Substring(2).Replace('/', '\\'))));
                                streamBinaryWriter.Flush();
                                socket.Close();
                                break;
                            }
                        case 2:
                            {
                                var (size, bytes) = Get(strings[1].Substring(2).Replace('/', '\\'));
                                var bytes_massive = Encoding.UTF8.GetBytes(size.ToString());
                                streamBinaryWriter.Write(bytes);
                                streamBinaryWriter.Flush();
                                socket.Close();
                                break;
                            }
                        default:
                            throw new ArgumentException("Your key is out of index");
                    }
                });
            }
        }
    }
}
