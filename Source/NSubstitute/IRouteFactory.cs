namespace NSubstitute
{
    public interface IRouteFactory
    {
        IRoute Create<TRoute>(params object[] routeArguments) where TRoute : IRoute;
    }
}