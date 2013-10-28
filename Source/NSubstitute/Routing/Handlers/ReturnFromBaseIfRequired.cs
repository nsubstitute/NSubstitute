using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromBaseIfRequired : ICallHandler
    {
        private readonly bool _required;

        public ReturnFromBaseIfRequired(bool required)
        {
            _required = required;
        }

        public RouteAction Handle(ICall call)
        {
            if (!_required) return RouteAction.Continue();

            return call
                    .TryCallBase()
                    .Fold(RouteAction.Continue, RouteAction.Return);
        }
    }
}