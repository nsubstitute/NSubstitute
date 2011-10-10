namespace NSubstitute.Core
{
    public class PendingSpecification : IPendingSpecification
    {
        ICallSpecification _pendingSpec;

        public bool HasPendingCallSpec()
        {
            return _pendingSpec != null;
        }

        public ICallSpecification UseCallSpec()
        {
            var specToUse = _pendingSpec;
            _pendingSpec = null;
            return specToUse;
        }

        public void Clear()
        {
            UseCallSpec();
        }

        public void Set(ICallSpecification callSpecification)
        {
            _pendingSpec = callSpecification;
        }
    }
}