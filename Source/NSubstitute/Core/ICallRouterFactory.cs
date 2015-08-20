namespace NSubstitute.Core
{
    public interface ICallRouterFactory
    {
        ICallRouter Create(ISubstitutionContext substitutionContext, ISubstituteState substituteState);
    }
}