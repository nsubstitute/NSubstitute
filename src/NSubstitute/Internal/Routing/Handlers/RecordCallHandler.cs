using NSubstitute.Core;
using NSubstitute.Internal.Core;

namespace NSubstitute.Internal.Routing.Handlers;

public class RecordCallHandler(ICallCollection callCollection, SequenceNumberGenerator generator) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        call.AssignSequenceNumber(generator.Next());
        callCollection.Add(call);

        return RouteAction.Continue();
    }
}