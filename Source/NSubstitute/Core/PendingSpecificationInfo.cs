namespace NSubstitute.Core
{
    public class PendingSpecificationInfo
    {
        public ICallSpecification CallSpecification { get; }
        public ICall LastCall { get; }

        private PendingSpecificationInfo(ICallSpecification callSpecification, ICall lastCall)
        {
            CallSpecification = callSpecification;
            LastCall = lastCall;
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