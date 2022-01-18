﻿namespace MyFTP
{
    /// <summary>
    /// Contating the information about the information got from the server
    /// </summary>
    public class GetResponseStruct
    {
        /// <summary>
        /// Data in bytes received by tcp.
        /// </summary>
        public byte[]? Data { get; set; }

        /// <summary>
        /// Message size.
        /// </summary>
        public long Size() => Data.Length;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResponseStruct"/> class.
        /// Default constructor.
        /// </summary>
        public GetResponseStruct() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResponseStruct"/> class.
        /// Constructor for GetResponse, conains data about the lenght and the content of the message.
        /// </summary>
        public GetResponseStruct(byte[] bytes)
        {
            Data = bytes;
        }
    }
}
