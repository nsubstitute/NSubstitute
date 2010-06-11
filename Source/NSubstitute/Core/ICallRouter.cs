namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        void LastCallShouldReturn(IReturn returnValue);
        object Route(ICall call);
        void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute;
        void ClearReceivedCalls();
    }
}