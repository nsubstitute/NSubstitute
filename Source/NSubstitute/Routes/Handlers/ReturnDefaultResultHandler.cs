namespace NSubstitute.Routes.Handlers
{
    public class ReturnDefaultResultHandler : ICallHandler
    {
        private readonly ICallResults _callResults;

        public ReturnDefaultResultHandler(ICallResults callResults)
        {
            _callResults = callResults;
        }

        public object Handle(ICall call)
        {
            return _callResults.GetDefaultResultFor(call);
        }
    }
}