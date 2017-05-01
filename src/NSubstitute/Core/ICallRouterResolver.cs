namespace NSubstitute.Core
{
    public interface ICallRouterResolver
    {
        void Register(object proxy, ICallRouter callRouter);
        ICallRouter ResolveFor(object substitute);
    }
}