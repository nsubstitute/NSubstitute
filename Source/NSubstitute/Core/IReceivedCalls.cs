namespace NSubstitute.Core
{
    public interface IReceivedCalls
    {
        void ThrowIfCallNotFound(ICallSpecification callSpecification);
    }
}