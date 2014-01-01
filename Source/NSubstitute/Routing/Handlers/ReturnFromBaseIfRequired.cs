using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromBaseIfRequired : ICallHandler
    {
        private readonly bool _required;
        private readonly ICallBaseExclusions _callBaseExclusions;

        public ReturnFromBaseIfRequired(bool required, ICallBaseExclusions callBaseExclusions)
        {
            _required = required;
            _callBaseExclusions = callBaseExclusions;
        }

        public RouteAction Handle(ICall call)
        {
            if (!_required) return RouteAction.Continue();
            if (_callBaseExclusions.IsExcluded(call)) return RouteAction.Continue();

            return call
                    .TryCallBase()
                    .Fold(RouteAction.Continue, RouteAction.Return);
        }
    }
}