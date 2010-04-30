using System;

namespace NSubstitute
{
    public class DoWhenCalledRoute : IRoute
    {
        public DoWhenCalledRoute(IRouteParts routeParts)
        {
        }

        public object Handle(ICall call)
        {
            return null;
        }
    }
}