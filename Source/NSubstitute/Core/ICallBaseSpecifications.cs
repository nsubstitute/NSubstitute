namespace NSubstitute.Core
{
    public interface ICallBaseSpecifications
    {
        void Add(ICallSpecification callSpecification);
        bool DoesCallBase(ICall callInfo);
    }
}