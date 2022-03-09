// <copyright file="ResponseFormat.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyFTP;

/// <summary>
/// Struct to contain the information about each file.
/// </summary>
public class ResponseFormat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseFormat"/> class.
    /// Constructor for item from response.
    /// </summary>
    public ResponseFormat(string name, string isDir)
    {
        Name = name;
        IsDir = bool.Parse(isDir);
    }

    /// <summary>
    /// Contains the imformation about the name of the file/dir.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Contains the imformation about whether this file is Directory.
    /// </summary>
    public bool IsDir { get; }
}