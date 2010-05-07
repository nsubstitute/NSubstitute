namespace NSubstitute.Core
{
    public interface IArgumentSpecification
    {
        bool IsSatisfiedBy(object argument);
    }
}