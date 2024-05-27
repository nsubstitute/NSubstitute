using System.Collections;

namespace NSubstitute.Core.Arguments;

public class ArrayContentsArgumentMatcher(IEnumerable<IArgumentSpecification> argumentSpecifications) : IArgumentMatcher, IArgumentFormatter
{
    private readonly IArgumentSpecification[] _argumentSpecifications = argumentSpecifications.ToArray();

    public bool IsSatisfiedBy(object? argument)
    {
        if (argument != null)
        {
            var argumentArray = ((IEnumerable)argument).Cast<object>().ToArray();
            if (argumentArray.Length == _argumentSpecifications.Length)
            {
                return _argumentSpecifications
                    .Select((spec, index) => spec.IsSatisfiedBy(argumentArray[index]))
                    .All(x => x);
            }
        }

        return false;
    }

    public override string ToString() => string.Join(", ", _argumentSpecifications.Select(x => x.ToString()));

    public string Format(object? argument, bool highlight)
    {
        var argArray = argument is IEnumerable enumerableArgs ? enumerableArgs.Cast<object>().ToArray() : [];
        return Format(argArray, _argumentSpecifications).Join(", ");
    }

    private IEnumerable<string> Format(object[] args, IArgumentSpecification[] specs)
    {
        if (specs.Any() && !args.Any())
        {
            return new[] { "**" };
        }
        return args.Select((arg, index) =>
        {
            var hasSpecForThisArg = index < specs.Length;
            return hasSpecForThisArg ? specs[index].FormatArgument(arg) : ArgumentFormatter.Default.Format(arg, true);
        });
    }
}