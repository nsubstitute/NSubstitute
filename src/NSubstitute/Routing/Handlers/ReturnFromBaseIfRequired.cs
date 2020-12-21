using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromBaseIfRequired : ICallHandler
    {
        private readonly ICallBaseConfiguration _config;

        public ReturnFromBaseIfRequired(ICallBaseConfiguration config)
        {
            _config = config;
        }

        public RouteAction Handle(ICall call)
        {
            if (_config.ShouldCallBase(call))
            {
                return call
                    .TryCallBase()
                    .Fold(RouteAction.Continue, RouteAction.Return);
            }

            return RouteAction.Continue();
        }
    }
}