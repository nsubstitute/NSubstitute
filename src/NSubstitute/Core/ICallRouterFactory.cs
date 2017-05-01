namespace NSubstitute.Core
{
    public interface ICallRouterFactory
    {
        ICallRouter Create(ISubstitutionContext substitutionContext, SubstituteConfig config);
    }
}