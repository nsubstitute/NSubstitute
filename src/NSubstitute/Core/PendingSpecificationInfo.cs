using System;

namespace NSubstitute.Core
{
    public class PendingSpecificationInfo
    {
        private readonly bool _hasCallSpec;
        private readonly ICallSpecification _callSpecification;
        private readonly ICall _lastCall;

        private PendingSpecificationInfo(bool hasCallSpec, ICallSpecification callSpecification, ICall lastCall)
        {
            _hasCallSpec = hasCallSpec;
            _callSpecification = callSpecification;
            _lastCall = lastCall;
        }

        public T Handle<T>(Func<ICallSpecification, T> onCallSpec, Func<ICall, T> onLastCall)
        {
            return _hasCallSpec ? onCallSpec(_callSpecification) : onLastCall(_lastCall);
        }

        public static PendingSpecificationInfo FromLastCall(ICall lastCall)
        {
            return new PendingSpecificationInfo(false, null, lastCall);
        }

        public static PendingSpecificationInfo FromCallSpecification(ICallSpecification callSpecification)
        {
            return new PendingSpecificationInfo(true, callSpecification, null);
        }
    }
}