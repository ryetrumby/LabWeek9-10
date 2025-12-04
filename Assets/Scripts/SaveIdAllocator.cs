using System.Threading;

public static class SaveIdAllocator
{
    static int counter = 0;
    public static int NextId() => Interlocked.Increment(ref counter);
}
