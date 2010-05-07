namespace NSubstitute.Core
{
    public interface ICallHandler
    {
        object Handle(ICall call);
    }
}