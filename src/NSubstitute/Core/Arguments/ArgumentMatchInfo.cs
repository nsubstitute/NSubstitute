namespace NSubstitute.Core.Arguments;

public record ArgumentMatchInfo
{
    private readonly object? _argument;
    private readonly IArgumentSpecification _specification;

    public int Index { get; }

    public ArgumentMatchInfo(int index, object? argument, IArgumentSpecification specification)
    {
        _argument = argument;
        _specification = specification;
        Index = index;
    }

    public bool IsMatch => _specification.IsSatisfiedBy(_argument);

    public string DescribeNonMatch()
    {
        var describeNonMatch = _specification.DescribeNonMatch(_argument);
        if (string.IsNullOrEmpty(describeNonMatch)) return string.Empty;
        var argIndexPrefix = "arg[" + Index + "]: ";
        return string.Format("{0}{1}", argIndexPrefix, describeNonMatch.Replace("\n", "\n".PadRight(argIndexPrefix.Length + 1)));
    }
}