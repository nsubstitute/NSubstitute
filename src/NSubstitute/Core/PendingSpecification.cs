using System;

namespace NSubstitute.Core
{
    public class PendingSpecification : IPendingSpecification
    {
        private readonly ISubstitutionContext _substitutionContext;

        public PendingSpecification(ISubstitutionContext substitutionContext)
        {
            _substitutionContext = substitutionContext;
        }

        public bool HasPendingCallSpecInfo()
        {
            return _substitutionContext.PendingSpecificationInfo != null;
        }

        public PendingSpecificationInfo UseCallSpecInfo()
        {
            var info = _substitutionContext.PendingSpecificationInfo;
            Clear();
            return info;
        }

        public void SetCallSpecification(ICallSpecification callSpecification)
        {
            _substitutionContext.PendingSpecificationInfo = PendingSpecificationInfo.FromCallSpecification(callSpecification);
        }

        public void SetLastCall(ICall lastCall)
        {
            _substitutionContext.PendingSpecificationInfo = PendingSpecificationInfo.FromLastCall(lastCall);
        }

        public void Clear()
        {
            _substitutionContext.PendingSpecificationInfo = null;
        }
    }
}