namespace NSubstitute.Core
{
    public interface ICallHandler
    {
        RouteAction Handle(ICall call);
    }
}