namespace NSubstitute.Core
{
    public interface IEventRaiser
    {
        void Raise(ICall call, object[] argumentsToRaiseEventWith);
    }
}