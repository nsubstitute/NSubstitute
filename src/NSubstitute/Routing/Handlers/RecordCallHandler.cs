using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class RecordCallHandler(ICallCollection callCollection, SequenceNumberGenerator generator) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        call.AssignSequenceNumber(generator.Next());
        callCollection.Add(call);

        return RouteAction.Continue();
    }
}