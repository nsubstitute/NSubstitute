namespace NSubstitute
{
    public interface ICallHandler
    {
        void LastCallShouldReturn<T>(T valueToReturn);
        object Handle(ICall call);
        void AssertNextCallHasBeenReceived();
    }
}