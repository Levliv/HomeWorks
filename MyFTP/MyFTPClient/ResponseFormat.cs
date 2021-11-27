namespace MyFTP;
/// <summary>
/// Struct to contain the information about each file
/// </summary>
public class ResponseFormat
{
    /// <summary>
    /// Contains the imformation about the name of the file/dir
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Contains the imformation about whether this file is Directory
    /// </summary>
    public bool IsDir { get; set; }
}