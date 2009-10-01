namespace NSubstitute
{
    public interface IInvocationHandlerFactory
    {
        IInvocationHandler CreateInvocationHandler(ISubstitutionContext substitutionContext);
    }
}