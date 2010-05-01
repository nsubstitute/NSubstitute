namespace NSubstitute.Routes.Handlers
{
    public class ReturnDefaultForCallHandler : ICallHandler
    {
        private readonly ICallResults _callResults;

        public ReturnDefaultForCallHandler(ICallResults callResults)
        {
            _callResults = callResults;
        }

        public object Handle(ICall call)
        {
            return _callResults.GetDefaultResultFor(call);
        }
    }
}