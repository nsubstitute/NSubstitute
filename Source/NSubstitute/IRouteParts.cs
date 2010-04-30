namespace NSubstitute
{
    public interface IRouteParts
    {
        ICallHandler GetPart<TPart>() where TPart : ICallHandler;
    }
}