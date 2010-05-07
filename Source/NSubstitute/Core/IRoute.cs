namespace NSubstitute.Core
{
    public interface IRoute
    {
        object Handle(ICall call);
    }
}