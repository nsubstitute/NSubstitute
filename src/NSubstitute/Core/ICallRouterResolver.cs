namespace NSubstitute.Core
{
    public interface ICallRouterResolver
    {
        ICallRouter ResolveFor(object substitute);
    }
}