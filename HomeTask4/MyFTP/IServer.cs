using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP
{
    interface IServer
    {
        public (int size, string name, bool isDir) List(string path);
        public (int size, byte[] content) Get(string path);
    }
}
