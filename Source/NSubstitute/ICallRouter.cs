using System;

namespace NSubstitute
{
    public interface ICallRouter
    {
        void LastCallShouldReturn<T>(T valueToReturn);
        object Route(ICall call);
        void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute;
    }
}