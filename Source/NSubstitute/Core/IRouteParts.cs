namespace NSubstitute.Core
{
    public interface IRouteParts
    {
        ICallHandler GetPart<TPart>() where TPart : ICallHandler;
    }
}