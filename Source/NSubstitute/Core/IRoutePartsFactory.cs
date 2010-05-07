namespace NSubstitute.Core
{
    public interface IRoutePartsFactory
    {
        IRouteParts Create(params object[] routeArguments);
    }
}