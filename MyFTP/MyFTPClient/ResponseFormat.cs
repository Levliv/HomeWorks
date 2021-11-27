namespace MyFTPClient;

/// <summary>
/// Struct to contain the information about each file
/// </summary>
public class ResponseFormat
{
    /// <summary>
    /// Constructor for item from response
    /// </summary>
    public ResponseFormat(string name, string isDir)
    {
        Name = name;
        IsDir = bool.Parse(isDir);
    }

    /// <summary>
    /// Contains the imformation about the name of the file/dir
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Contains the imformation about whether this file is Directory
    /// </summary>
    public bool IsDir { get; set; }
}