namespace NSubstitute
{
    public interface IRoute
    {
        object Handle(ICall call);
    }
}