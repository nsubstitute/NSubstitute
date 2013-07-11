using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnBaseResultByDefaultCallHandler : ICallHandler
    {
        private readonly ISubstituteState _substituteState;

        public ReturnBaseResultByDefaultCallHandler(ISubstituteState substituteState)
        {
            _substituteState = substituteState;
        }

        public RouteAction Handle(ICall call)
        {
            if (_substituteState.CallBaseByDefault)
            {
                return RouteAction.Return(call.CallBase());
            }

            return RouteAction.Continue();
        }
    }
}