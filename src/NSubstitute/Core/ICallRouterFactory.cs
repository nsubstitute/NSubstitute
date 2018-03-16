namespace NSubstitute.Core
{
    public interface ICallRouterFactory
    {
        ICallRouter Create(SubstituteConfig config, IThreadLocalContext threadContext, ISubstituteFactory substituteFactory);
    }
}