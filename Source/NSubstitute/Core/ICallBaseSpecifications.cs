namespace NSubstitute.Core
{
    public interface ICallBaseSpecifications
    {
        void Add(ICallSpecification callSpecification);
        void Remove(ICallSpecification callSpecification);
        bool DoesCallBase(ICall callInfo);
    }
}