namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn<T>(T value);
        void LastCallHandler(ICallHandler callHandler);
        ISubstituteFactory GetSubstituteFactory();
        ICallHandler GetCallHandlerFor(object substitute);
    }
}