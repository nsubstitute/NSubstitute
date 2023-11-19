namespace NSubstitute.Core.Arguments
{
    public interface IArgumentFormatter
    {
        string Format(object? arg, bool highlight);
    }
}