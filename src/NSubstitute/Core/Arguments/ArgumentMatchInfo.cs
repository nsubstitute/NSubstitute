namespace NSubstitute.Core.Arguments;

public class ArgumentMatchInfo(int index, object? argument, IArgumentSpecification specification)
{
    private readonly object? _argument = argument;
    private readonly IArgumentSpecification _specification = specification;
    public int Index { get; } = index;

    public bool IsMatch => _specification.IsSatisfiedBy(_argument);

    public string DescribeNonMatch()
    {
        var describeNonMatch = _specification.DescribeNonMatch(_argument);
        if (string.IsNullOrEmpty(describeNonMatch)) return string.Empty;
        var argIndexPrefix = "arg[" + Index + "]: ";
        return string.Format("{0}{1}", argIndexPrefix, describeNonMatch.Replace("\n", "\n".PadRight(argIndexPrefix.Length + 1)));
    }

    public bool Equals(ArgumentMatchInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return other.Index == Index && Equals(other._argument, _argument) && Equals(other._specification, _specification);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(ArgumentMatchInfo)) return false;
        return Equals((ArgumentMatchInfo)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int result = Index;
            result = (result * 397) ^ (_argument != null ? _argument.GetHashCode() : 0);
            result = (result * 397) ^ _specification.GetHashCode();
            return result;
        }
    }
}