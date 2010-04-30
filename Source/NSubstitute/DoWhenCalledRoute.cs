using System;

namespace NSubstitute
{
    public class DoWhenCalledRoute : IRoute
    {
        private readonly IRouteParts _routeParts;

        public DoWhenCalledRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<SetActionForCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultForCallHandler>().Handle(call);
        }
    }
}