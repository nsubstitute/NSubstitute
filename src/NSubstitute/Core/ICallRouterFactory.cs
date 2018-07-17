namespace NSubstitute.Core
{
    public interface ICallRouterFactory
    {
        ICallRouter Create(ISubstituteState substituteState, bool canConfigureBaseCalls);
    }
}