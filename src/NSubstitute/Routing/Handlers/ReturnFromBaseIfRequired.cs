using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromBaseIfRequired : ICallHandler
    {
        private readonly ICallBaseConfiguration _config;

        public ReturnFromBaseIfRequired(ICallBaseConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public RouteAction Handle(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

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