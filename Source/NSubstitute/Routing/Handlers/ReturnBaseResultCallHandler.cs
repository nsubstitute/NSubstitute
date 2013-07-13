using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnBaseResultCallHandler : ICallHandler
    {
        private readonly ISubstituteState _substituteState;
        private readonly ICallBaseSpecifications _callBaseSpecifications;

        public ReturnBaseResultCallHandler(ISubstituteState substituteState, ICallBaseSpecifications callBaseSpecifications)
        {
            _substituteState = substituteState;
            _callBaseSpecifications = callBaseSpecifications;
        }

        public RouteAction Handle(ICall call)
        {
            if (_callBaseSpecifications.DoesCallBase(call) || _substituteState.CallBaseByDefault)
            {
                return RouteAction.Return(call.CallBase());
            }

            return RouteAction.Continue();
        }
    }
}