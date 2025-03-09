using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

internal sealed class RecordCallHandler(ICallCollection callCollection, SequenceNumberGenerator generator) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        call.AssignSequenceNumber(generator.Next());
        callCollection.Add(call);

        return RouteAction.Continue();
    }
}