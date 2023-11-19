using System;

namespace NSubstitute.Core
{
    public class PendingSpecificationInfo
    {
        private readonly ICallSpecification? _callSpecification;
        private readonly ICall? _lastCall;

        private PendingSpecificationInfo(ICallSpecification? callSpecification, ICall? lastCall)
        {
            _callSpecification = callSpecification;
            _lastCall = lastCall;
        }

        public T Handle<T>(Func<ICallSpecification, T> onCallSpec, Func<ICall, T> onLastCall)
        {
            return _callSpecification != null ? onCallSpec(_callSpecification) : onLastCall(_lastCall!);
        }

        public static PendingSpecificationInfo FromLastCall(ICall lastCall)
        {
            return new PendingSpecificationInfo(null, lastCall);
        }

        public static PendingSpecificationInfo FromCallSpecification(ICallSpecification callSpecification)
        {
            return new PendingSpecificationInfo(callSpecification, null);
        }
    }
}