using System;

namespace NSubstitute
{
    public interface IRoutePartsFactory
    {
        IRouteParts Create(params object[] routeArguments);
    }
}