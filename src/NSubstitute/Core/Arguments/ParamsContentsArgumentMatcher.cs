namespace NSubstitute.Core.Arguments;

public class ParamsContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications) : IArgumentMatcher, IArgumentFormatter
{
    private readonly IArgumentSpecification[] _argumentSpecifications = argumentSpecifications.ToArray();

    public bool IsSatisfiedBy(object? argument)
    {
        if (argument != null)
        {
            var argumentValues = ParamsSupport.UnwrapArgument(argument).ToArray();
            if (argumentValues.Length == _argumentSpecifications.Length)
            {
                return _argumentSpecifications
                    .Select((spec, index) => spec.IsSatisfiedBy(argumentValues[index]))
                    .All(x => x);
            }
        }

        return false;
    }

    public override string ToString() => string.Join(", ", _argumentSpecifications.Select(x => x.ToString()));

    public string Format(object? argument, bool highlight)
    {
        ParamsSupport.TryUnwrapArgument(argument, out var argumentSequence);
        var argumentValues = argumentSequence.ToArray();
        return Format(argumentValues, _argumentSpecifications).Join(", ");
    }

    private IEnumerable<string> Format(object?[] argumentValues, IArgumentSpecification[] specs)
    {
        if (specs.Any() && !argumentValues.Any())
        {
            return new[] { "**" };
        }
        return argumentValues.Select((arg, index) =>
        {
            var hasSpecForThisArg = index < specs.Length;
            return hasSpecForThisArg ? specs[index].FormatArgument(arg) : ArgumentFormatter.Default.Format(arg, true);
        });
    }
}