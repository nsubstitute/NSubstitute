namespace NSubstitute.Core
{
    public interface ICallBaseExclusions
    {
        void Exclude(ICallSpecification callSpecification);
        bool IsExcluded(ICall callInfo);
    }
}