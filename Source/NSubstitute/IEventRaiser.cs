namespace NSubstitute
{
    public interface IEventRaiser
    {
        void Raise(ICall call, object[] argumentsToRaiseEventWith);
    }
}