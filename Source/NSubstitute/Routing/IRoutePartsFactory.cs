namespace NSubstitute.Routing
{
    public interface IRoutePartsFactory
    {
        IRouteParts Create(params object[] routeArguments);
    }
}