// <copyright file="GetResponseStruct.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyFTP;

/// <summary>
/// Containing the information about the information got from the server.
/// </summary>
public class GetResponseStruct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResponseStruct"/> class.
    /// Default constructor.
    /// </summary>
    public GetResponseStruct()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetResponseStruct"/> class.
    /// Constructor for GetResponse, contains data about the lenght and the content of the message.
    /// </summary>
    public GetResponseStruct(byte[] bytes)
    {
        Data = bytes;
    }

    /// <summary>
    /// Data in bytes.
    /// </summary>
    private byte[]? Data { get; set; }

    /// <summary>
    /// Message size.
    /// </summary>
    /// <returns> File size or -1 if file wasn't found. </returns>
    public int Size()
    {
        if (Data != null)
        {
            return Data.Length;
        }
        return -1;
    }
}
