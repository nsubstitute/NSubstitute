using NSubstitute.Internal.Core;

namespace NSubstitute.Core;

public interface IPendingSpecification
{
    bool HasPendingCallSpecInfo();
    PendingSpecificationInfo? UseCallSpecInfo();
    void SetCallSpecification(ICallSpecification callSpecification);
    void SetLastCall(ICall lastCall);
    void Clear();
}