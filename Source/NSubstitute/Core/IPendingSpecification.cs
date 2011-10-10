namespace NSubstitute.Core
{
    public interface IPendingSpecification
    {
        bool HasPendingCallSpec();
        ICallSpecification UseCallSpec();
        void Set(ICallSpecification callSpecification);
        void Clear();
    }
}