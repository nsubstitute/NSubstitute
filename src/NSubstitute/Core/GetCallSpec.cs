namespace NSubstitute.Core;

public class GetCallSpec(
    ICallCollection receivedCalls,
    ICallSpecificationFactory callSpecificationFactory,
    ICallActions callActions) : IGetCallSpec
{
    public ICallSpecification FromPendingSpecification(MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
    {
        return pendingSpecInfo.Handle(
            callSpec => FromExistingSpec(callSpec, matchArgs),
            lastCall =>
            {
                receivedCalls.Delete(lastCall);
                return FromCall(lastCall, matchArgs);
            });
    }

    public ICallSpecification FromCall(ICall call, MatchArgs matchArgs)
    {
        return callSpecificationFactory.CreateFrom(call, matchArgs);
    }

    public ICallSpecification FromExistingSpec(ICallSpecification spec, MatchArgs matchArgs)
    {
        return matchArgs == MatchArgs.AsSpecifiedInCall ? spec : UpdateCallSpecToMatchAnyArgs(spec);
    }

    private ICallSpecification UpdateCallSpecToMatchAnyArgs(ICallSpecification callSpecification)
    {
        var anyArgCallSpec = callSpecification.CreateCopyThatMatchesAnyArguments();
        callActions.MoveActionsForSpecToNewSpec(callSpecification, anyArgCallSpec);
        return anyArgCallSpec;
    }
}