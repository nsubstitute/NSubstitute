using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ClearUnusedCallSpecHandler : ICallHandler
    {
        private readonly ISubstituteState _state;

        public ClearUnusedCallSpecHandler(ISubstituteState state)
        {
            _state = state;
        }

        public RouteAction Handle(ICall call)
        {
            _state.ClearUnusedCallSpecs();
            return RouteAction.Continue();
        }
    }
}