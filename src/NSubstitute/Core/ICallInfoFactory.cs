namespace NSubstitute.Core
{
    public interface ICallInfoFactory
    {
        CallInfo Create(ICall call);
    }
}