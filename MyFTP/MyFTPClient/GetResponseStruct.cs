﻿namespace MyFTPClient
{
    /// <summary>
    /// Contating the information about the information got from the server
    /// </summary>
    public class GetResponseStruct
    {
        /// <summary>
        /// Data in bytes received by tcp
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Message size
        /// </summary>
        public long Size() => Data.Length;

        /// <summary>
        /// Constructor for GetResponse, conains data about the lenght and the content of the message
        /// </summary>
        public GetResponseStruct(byte[] bytes)
        {
            Data = bytes;
        }
    }
}