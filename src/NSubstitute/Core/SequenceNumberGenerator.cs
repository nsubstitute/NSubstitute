using System.Threading;

namespace NSubstitute.Core
{
    public class SequenceNumberGenerator
    {
        private long current = long.MinValue;

        public virtual long Next()
        {
            return Interlocked.Increment(ref current);
        }
    }
}