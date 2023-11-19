using System;

namespace NSubstitute.Core
{
    public class GetCallSpec : IGetCallSpec
    {
        private readonly ICallCollection _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallActions _callActions;

        public GetCallSpec(ICallCollection receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallActions callActions)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _callActions = callActions;
        }

        public ICallSpecification FromPendingSpecification(MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
        {
            return pendingSpecInfo.Handle(
                callSpec => FromExistingSpec(callSpec, matchArgs),
                lastCall =>
                {
                    _receivedCalls.Delete(lastCall);
                    return FromCall(lastCall, matchArgs);
                });
        }

        public ICallSpecification FromCall(ICall call, MatchArgs matchArgs)
        {
            return _callSpecificationFactory.CreateFrom(call, matchArgs);
        }

        public ICallSpecification FromExistingSpec(ICallSpecification spec, MatchArgs matchArgs)
        {
            return matchArgs == MatchArgs.AsSpecifiedInCall ? spec : UpdateCallSpecToMatchAnyArgs(spec);
        }

        private ICallSpecification UpdateCallSpecToMatchAnyArgs(ICallSpecification callSpecification)
        {
            var anyArgCallSpec = callSpecification.CreateCopyThatMatchesAnyArguments();
            _callActions.MoveActionsForSpecToNewSpec(callSpecification, anyArgCallSpec);
            return anyArgCallSpec;
        }
    }
}