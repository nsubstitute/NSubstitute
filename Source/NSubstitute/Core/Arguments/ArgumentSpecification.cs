using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        static readonly Action<object> NoOpAction = x => { };
        readonly Type _forType;
        readonly IArgumentMatcher _matcher;
        readonly IArgumentMatcher _compatibleTypeMatcher;
        readonly Action<object> _action;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher) : this(forType, matcher, NoOpAction) { }

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher, Action<object> action)
        {
            _forType = forType;
            _matcher = matcher;
            _compatibleTypeMatcher = new AnyArgumentMatcher(forType);
            _action = action;
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (!IsCompatibleType(argument)) return false;
            try { return _matcher.IsSatisfiedBy(argument); }
            catch { return false; }
        }

        public string DescribeNonMatch(object argument)
        {
            var describable = _matcher as IDescribeNonMatches;
            return describable == null ? string.Empty : describable.DescribeFor(argument);
        }

        public Type ForType { get { return _forType; } }

        public override string ToString() { return _matcher.ToString(); }

        public IArgumentSpecification CreateCopyMatchingAnyArgOfType(Type requiredType)
        {
            return new ArgumentSpecification(requiredType, new AnyArgumentMatcher(requiredType), RunActionIfTypeIsCompatible);
        }

        public void RunAction(object argument)
        {
            _action(argument);
        }

        private bool IsCompatibleType(object argument)
        {
            return _compatibleTypeMatcher.IsSatisfiedBy(argument);
        }

        private void RunActionIfTypeIsCompatible(object argument)
        {
            if (!argument.IsCompatibleWith(ForType)) return;
            _action(argument);
        }
    }
}