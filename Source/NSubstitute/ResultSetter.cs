using System;

namespace NSubstitute
{
    public class ResultSetter : IResultSetter
    {
        private readonly ICallStack _callStack;
        private readonly ICallResults _configuredResults;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public ResultSetter(ICallStack callStack, ICallResults configuredResults, ICallSpecificationFactory callSpecificationFactory)
        {
            _callStack = callStack;
            _configuredResults = configuredResults;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public void SetResultForLastCall<T>(T valueToReturn)
        {
            var lastCall = _callStack.Pop();
            var callSpecification = _callSpecificationFactory.CreateFrom(lastCall);
            _configuredResults.SetResult(callSpecification, valueToReturn);
        }
    }
}