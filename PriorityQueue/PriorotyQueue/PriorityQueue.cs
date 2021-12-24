namespace PriorityQueue;

/// <summary>
/// Blocking queue
/// </summary>
public class PriorityQueue
{ 
    private object locker = new object();
    private int length = 0;

    /// <summary>
    /// Queue head
    /// </summary>
    public Node head { get; set; } = null;

    public int Size() => length;

    /// <summary>
    /// Enqeueue blocking queue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="priority"></param>
    /// <returns>True if operation was successful</returns>
    public bool Enqueue(int value, int priority)
    {
        try
        {
            var newNode = new Node(value, priority);
            lock (locker)
            {
                if (length == 0)
                {
                    head = newNode;
                    Interlocked.Increment(ref length);
                    return true;
                }
                if (head.Priority > priority)
                {
                    head.Next = newNode;
                    Interlocked.Increment(ref length);
                    return true;

                }
                else
                {
                    newNode.Next = head;
                    head = newNode;
                    Interlocked.Increment(ref length);
                    return true;
                }
                var currentNode = head;
                for (int i = 0; i < length; i++)
                {
                    if (currentNode.Next == null)
                    {
                        break;
                    }
                    if (currentNode.Next.Priority < priority)
                    {
                        break;
                    }
                }
                var tmpNode = currentNode.Next;
                currentNode.Next = newNode;
                newNode.Next = tmpNode;
                Interlocked.Increment(ref length);
                Monitor.PulseAll(locker);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured {ex.Message}");
            return false;
        }
    }
    public int Dequeue()
    {
        lock (locker)
        {
            while (length == 0)
            {
                Monitor.Wait(locker);
            }
            var value = head.Value;
            head = head.Next;
            Interlocked.Decrement(ref length);
            Monitor.PulseAll(locker);
            return value;
        }
    }
}