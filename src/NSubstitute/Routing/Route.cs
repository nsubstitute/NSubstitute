using NSubstitute.Core;

namespace NSubstitute.Routing;

public class Route(ICallHandler[] handlers) : IRoute
{
    public IEnumerable<ICallHandler> Handlers => handlers;

    public object? Handle(ICall call)
    {
        // This is a hot method which is invoked frequently and has major impact on performance.
        // Therefore, the LINQ cycle was unwinded to for loop.
        for (int i = 0; i < handlers.Length; i++)
        {
            var result = handlers[i].Handle(call);
            if (result.HasReturnValue)
            {
                return result.ReturnValue;
            }
        }

        return null;
    }
}