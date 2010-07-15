namespace NSubstitute.Routing
{
    public interface IRoutePartsFactory
    {
        ICallHandlerFactory Create(params object[] routeArguments);
    }
}