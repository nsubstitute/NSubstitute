namespace NSubstitute.Core
{
    public interface ICallCollection
    {
        void Add(ICall call);
        void Delete(ICall call);
    }
}