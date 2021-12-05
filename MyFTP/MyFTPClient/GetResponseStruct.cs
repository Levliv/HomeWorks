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
        public long Size { get; }

        /// <summary>
        /// Data in bytes received by tcp
        /// </summary>
        public byte[] Data { get; }

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
