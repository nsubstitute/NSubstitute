using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        private static readonly Action<object?> NoOpAction = _ => { };

        private readonly IArgumentMatcher _matcher;
        private readonly Action<object?> _action;
        public Type ForType { get; }
        public bool HasAction => _action != NoOpAction;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher) : this(forType, matcher, NoOpAction) { }

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher, Action<object?> action)
        {
            ForType = forType;
            _matcher = matcher;
            _action = action;
        }

        public bool IsSatisfiedBy(object? argument)
        {
            if (!IsCompatibleWith(argument))
            {
                return false;
            }

            try
            {
                return _matcher.IsSatisfiedBy(argument);
            }
            catch
            {
                return false;
            }
        }

        public string DescribeNonMatch(object? argument)
        {
            if (!IsCompatibleWith(argument))
            {
                return GetIncompatibleTypeMessage(argument);
            }

            return _matcher is IDescribeNonMatches describe
                ? describe.DescribeFor(argument)
                : string.Empty;
        }

        public string FormatArgument(object? argument)
        {
            var isSatisfiedByArg = IsSatisfiedBy(argument);

            return _matcher is IArgumentFormatter matcherFormatter
                ? matcherFormatter.Format(argument, highlight: !isSatisfiedByArg)
                : ArgumentFormatter.Default.Format(argument, highlight: !isSatisfiedByArg);
        }

        public override string ToString() => _matcher.ToString() ?? string.Empty;

        public IArgumentSpecification CreateCopyMatchingAnyArgOfType(Type requiredType)
        {
            // Don't pass RunActionIfTypeIsCompatible method if no action is present.
            // Otherwise, unnecessary closure will keep reference to this and will keep it alive.
            return new ArgumentSpecification(
                requiredType,
                new AnyArgumentMatcher(requiredType),
                _action == NoOpAction ? NoOpAction : RunActionIfTypeIsCompatible);
        }

        public void RunAction(object? argument)
        {
            _action(argument);
        }

        private void RunActionIfTypeIsCompatible(object? argument)
        {
            if (argument.IsCompatibleWith(ForType))
            {
                _action(argument);
            }
        }

        private bool IsCompatibleWith(object? argument) => argument.IsCompatibleWith(ForType);

        private string GetIncompatibleTypeMessage(object? argument)
        {
            var argumentType = argument == null ? typeof(object) : argument.GetType();
            return $"Expected an argument compatible with type '{ForType}'. Actual type was '{argumentType}'.";
        }
    }
}