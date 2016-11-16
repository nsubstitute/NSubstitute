using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromCustomHandlers : ICallHandler
    {
        private readonly ICustomHandlers _customHandlers;

        public ReturnFromCustomHandlers(ICustomHandlers customHandlers)
        {
            _customHandlers = customHandlers;
        }
        public RouteAction Handle(ICall call)
        {
            foreach (var handler in _customHandlers.Handlers)
            {
                var result = handler.Handle(call);
                if (result.HasReturnValue)
                {
                    return result;
                }
            }

            return RouteAction.Continue();
        }
    }
}
