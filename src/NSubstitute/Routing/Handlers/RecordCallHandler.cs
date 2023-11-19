using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class RecordCallHandler : ICallHandler
    {
        private readonly ICallCollection _callCollection;
        private readonly SequenceNumberGenerator _generator;

        public RecordCallHandler(ICallCollection callCollection, SequenceNumberGenerator generator)
        {
            _callCollection = callCollection;
            _generator = generator;
        }

        public RouteAction Handle(ICall call)
        {
            call.AssignSequenceNumber(_generator.Next());
            _callCollection.Add(call);

            return RouteAction.Continue();
        }
    }
}