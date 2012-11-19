namespace NSubstitute.Core
{
    public interface ICallRouterProvider
    {
        ICallRouter CallRouter { get; }
    }
}