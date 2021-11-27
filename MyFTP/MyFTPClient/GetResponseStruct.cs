using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFTPClient
{
    /// <summary>
    /// Contating the information about the information got from the server
    /// </summary>
    public class GetResponseStruct
    {
        /// <summary>
        /// Message size
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Data in bytes recived got by tcp
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Constructor for GetResponse, conains data about the lenght and the content of the message
        /// </summary>
        public GetResponseStruct(long size, byte[] bytes)
        {
            Size = size;
            Data = bytes;
        }
    }
}
