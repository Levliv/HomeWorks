namespace PriorityQueue;

/// <summary>
/// Node of the Queue
/// </summary>
public class Node
{
    public Node(int value, int priority)
    {
        Value = value;
        Priority = priority;
    }
    public int Value { get; set; }
    public int Priority { get; set; }
    public Node? Next { get; set; }
}
