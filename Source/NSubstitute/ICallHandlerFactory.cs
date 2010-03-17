namespace NSubstitute
{
    public interface ICallHandlerFactory
    {
        ICallHandler CreateCallHandler(ISubstitutionContext substitutionContext);
    }
}