namespace NSubstitute.Core
{
    public interface ICallBaseSpecifications
    {
        void Add(ICallSpecification callSpec);
        bool DoesCallBase(ICall callInfo);
    }
}