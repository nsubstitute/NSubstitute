namespace NSubstitute.Core
{
    public interface ISubstituteFactory
    {
        T Create<T>() where T : class;
        ICallRouter GetCallRouterCreatedFor(object substitute);
    }
}