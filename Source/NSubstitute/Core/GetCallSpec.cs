namespace NSubstitute.Core
{
    public class GetCallSpec : IGetCallSpec
    {
        private readonly ICallStack _callStack;
        private readonly IPendingSpecification _pendingSpecification;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallActions _callActions;

        public GetCallSpec(ICallStack callStack, IPendingSpecification pendingSpecification,
            ICallSpecificationFactory callSpecificationFactory, ICallActions callActions)
        {
            _callStack = callStack;
            _pendingSpecification = pendingSpecification;
            _callSpecificationFactory = callSpecificationFactory;
            _callActions = callActions;
        }

        public ICallSpecification FromLastCall(MatchArgs matchArgs)
        {
            return _pendingSpecification.HasPendingCallSpec()
                               ? FromExistingSpec(_pendingSpecification.UseCallSpec(), matchArgs)
                               : FromCall(_callStack.Pop(), matchArgs);
        }

        public ICallSpecification FromCall(ICall call, MatchArgs matchArgs)
        {
            return _callSpecificationFactory.CreateFrom(call, matchArgs);
        }

        public ICallSpecification FromExistingSpec(ICallSpecification spec, MatchArgs matchArgs)
        {
            return matchArgs == MatchArgs.AsSpecifiedInCall ? spec : UpdateCallSpecToMatchAnyArgs(spec);
        }

        ICallSpecification UpdateCallSpecToMatchAnyArgs(ICallSpecification callSpecification)
        {
            var anyArgCallSpec = callSpecification.CreateCopyThatMatchesAnyArguments();
            _callActions.MoveActionsForSpecToNewSpec(callSpecification, anyArgCallSpec);
            return anyArgCallSpec;
        }
    }
}