namespace NSubstitute.Routing
{
    public interface IRouteFactory
    {
        IRoute Create<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition;
    }
}