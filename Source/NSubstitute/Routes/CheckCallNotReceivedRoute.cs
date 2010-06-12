using System;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;

namespace NSubstitute.Routes
{
    public class CheckCallNotReceivedRoute : IRoute
    {
        private readonly IRouteParts _routeParts;

        public CheckCallNotReceivedRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<CheckDidNotReceiveCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultResultHandler>().Handle(call);
        }
    }
}