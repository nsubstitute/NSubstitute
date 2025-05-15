namespace NSubstitute.Internal.Core;

public class SequenceNumberGenerator
{
    private long _current = long.MinValue;

    public virtual long Next()
    {
        return Interlocked.Increment(ref _current);
    }
}