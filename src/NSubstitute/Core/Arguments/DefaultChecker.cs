namespace NSubstitute.Core.Arguments;

public class DefaultChecker(IDefaultForType defaultForType) : IDefaultChecker
{
    public bool IsDefault(object? value, Type forType)
    {
        return EqualityComparer<object>.Default.Equals(value, defaultForType.GetDefaultFor(forType));
    }
}