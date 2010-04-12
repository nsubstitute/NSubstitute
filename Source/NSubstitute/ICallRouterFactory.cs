namespace NSubstitute
{
    public interface ICallRouterFactory
    {
        ICallRouter Create(ISubstitutionContext substitutionContext);
    }
}