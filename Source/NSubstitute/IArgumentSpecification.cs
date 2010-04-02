namespace NSubstitute
{
    public interface IArgumentSpecification
    {
        bool IsSatisfiedBy(object argument);
    }
}