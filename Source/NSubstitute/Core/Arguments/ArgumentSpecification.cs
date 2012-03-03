using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        static readonly Action<object> NoOpAction = x => { };
        readonly Type _forType;
        readonly IArgumentMatcher _matcher;
        readonly Action<object> _action;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher) : this(forType, matcher, NoOpAction) { }

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher, Action<object> action)
        {
            _forType = forType;
            _matcher = matcher;
            _action = action;
        }

        public IArgumentMatcher ArgumentMatcher
        {
            get { return _matcher; }
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (!IsCompatibleWith(argument)) return false;
            try { return _matcher.IsSatisfiedBy(argument); }
            catch { return false; }
        }

        public string DescribeNonMatch(object argument)
        {
            var describable = _matcher as IDescribeNonMatches;
            if (describable == null) return string.Empty;

            return IsCompatibleWith(argument) ? describable.DescribeFor(argument) : GetIncompatibleTypeMessage(argument);
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

        private void RunActionIfTypeIsCompatible(object argument)
        {
            if (!argument.IsCompatibleWith(ForType)) return;
            _action(argument);
        }

        private bool IsCompatibleWith(object argument)
        {
            return argument.IsCompatibleWith(ForType);
        }

        private string GetIncompatibleTypeMessage(object argument)
        {
            var argumentType = argument == null ? typeof(object) : argument.GetType();
            return string.Format("Expected an argument compatible with type {0}. Actual type was {1}.", ForType, argumentType);
        }
    }
}