namespace NSubstitute
{
    public interface ICallHandler
    {
        object Handle(ICall call);
    }
}