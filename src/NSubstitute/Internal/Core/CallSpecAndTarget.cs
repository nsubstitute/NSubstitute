using NSubstitute.Core;

namespace NSubstitute.Internal.Core;

public class CallSpecAndTarget(ICallSpecification callSpecification, object target)
{
    public ICallSpecification CallSpecification { get; } = callSpecification;
    public object Target { get; } = target;
}