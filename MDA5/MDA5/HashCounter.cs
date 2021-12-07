using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Concurrent;

namespace MyMD5
{
    internal static class HashCounter
    {
        public static byte[] ComputeHashSingleThread(string path)
        {
            return ComputeDirectoryHash(path);
        }
        public static byte[] ComputeHashMultiThread(string path)
        {
            return ComputeDirectoryHash(path);

        }
        private static byte[] ComputeDirectoryHashParallel(string path)
        {
            var dataDir = new BlockingCollection<byte[]>();
            var md5Hasher = MD5.Create();
            var directories = Directory.EnumerateDirectories(path).OrderBy(x => x);
            var dirName = path.Split('\\').Last();
            var totalByteOfHashes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(dirName));
            var tasks = new List<Task>();
            directories.AsParallel().ForAll(x => dataDir.Add(ComputeDirectoryHashParallel(x)));
            foreach (var dataHash in dataDir)
            {
                totalByteOfHashes = totalByteOfHashes.Concat(dataHash).ToArray();
            }
            var dataFile = new BlockingCollection<byte[]>();
            var files = Directory.EnumerateFiles(path).OrderBy(x => x);
            files.AsParallel().ForAll(x => dataFile.Add(ComputeFileHash(x)));
            foreach (var fileHash in dataFile)
            {
                totalByteOfHashes = totalByteOfHashes.Concat(fileHash).ToArray();
            }
            return md5Hasher.ComputeHash(totalByteOfHashes);
        }

        private static byte[] ComputeDirectoryHash(string path)
        {
            var md5Hasher = MD5.Create();
            var directories = Directory.EnumerateDirectories(path).OrderBy(x => x);
            var dirName = path.Split('\\').Last();
            var totalByteOfHashes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(dirName));
            foreach (var directory in directories)
            {
                var subDirHash = ComputeDirectoryHash(directory);
                totalByteOfHashes = totalByteOfHashes.Concat(subDirHash).ToArray();
            }
            var files = Directory.EnumerateFiles(path).OrderBy(x => x);
            foreach (var file in files)
            {
                var fileHash = ComputeFileHash(file);
                totalByteOfHashes = totalByteOfHashes.Concat(fileHash).ToArray();
            }
            return md5Hasher.ComputeHash(totalByteOfHashes);
        }

        private static byte[] ComputeFileHash(string path)
        {
            using var md5Hasher = MD5.Create();
            using var fileStream = File.OpenRead(path);
            var fileHashBytes = md5Hasher.ComputeHash(fileStream);
            return fileHashBytes;
        }
    }
}
