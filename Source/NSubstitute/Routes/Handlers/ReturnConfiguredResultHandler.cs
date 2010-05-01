using System;

namespace NSubstitute.Routes.Handlers
{
    public class ReturnConfiguredResultHandler : ICallHandler
    {
        private readonly ICallResults _callResults;

        public ReturnConfiguredResultHandler(ICallResults callResults)
        {
            _callResults = callResults;
        }

        public object Handle(ICall call)
        {
            return _callResults.GetResult(call);
        }
    }
}