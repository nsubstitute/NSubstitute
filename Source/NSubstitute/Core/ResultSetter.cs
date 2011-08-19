namespace NSubstitute.Core
{
    public class ResultSetter : IResultSetter
    {
        private readonly ICallStack _callStack;
        private readonly IPendingSpecification _pendingSpecification;
        private readonly ICallResults _configuredResults;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallActions _callActions;

        public ResultSetter(ICallStack callStack, IPendingSpecification pendingSpecification,
            ICallResults configuredResults, ICallSpecificationFactory callSpecificationFactory,
            ICallActions callActions)
        {
            _callStack = callStack;
            _pendingSpecification = pendingSpecification;
            _configuredResults = configuredResults;
            _callSpecificationFactory = callSpecificationFactory;
            _callActions = callActions;
        }

        public void SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs)
        {
            if (_pendingSpecification.HasPendingCallSpec())
            {
                SetResultForCall(_pendingSpecification.UseCallSpec(), valueToReturn, matchArgs);
            }
            else
            {
                var lastCall = _callStack.Pop();
                SetResultForCall(lastCall, valueToReturn, matchArgs);
            }
        }

        public void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call, matchArgs);
            _configuredResults.SetResult(callSpecification, valueToReturn);
        }

        void SetResultForCall(ICallSpecification callSpecification, IReturn valueToReturn, MatchArgs matchArgs)
        {
            var callSpecForReturnValue = GetCallSpecForArgMatchStrategy(callSpecification, matchArgs);
            _configuredResults.SetResult(callSpecForReturnValue, valueToReturn);
        }

        ICallSpecification GetCallSpecForArgMatchStrategy(ICallSpecification callSpecification, MatchArgs matchArgs)
        {
            return matchArgs == MatchArgs.AsSpecifiedInCall ? callSpecification : UpdateCallSpecToMatchAnyArgs(callSpecification);
        }

        ICallSpecification UpdateCallSpecToMatchAnyArgs(ICallSpecification callSpecification)
        {
            var anyArgCallSpec = callSpecification.CreateCopyThatMatchesAnyArguments();
            _callActions.MoveActionsForSpecToNewSpec(callSpecification, anyArgCallSpec);
            return anyArgCallSpec;
        }
    }
}