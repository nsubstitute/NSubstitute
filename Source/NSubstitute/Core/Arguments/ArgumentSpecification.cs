using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        readonly Type _forType;
        readonly IArgumentMatcher _matcher;
        readonly IArgumentMatcher _compatibleTypeMatcher;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher)
        {
            _forType = forType;
            _matcher = matcher;
            _compatibleTypeMatcher = new AnyArgumentMatcher(forType);
            Action = x => { };
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (!_compatibleTypeMatcher.IsSatisfiedBy(argument)) return false;
            try { return _matcher.IsSatisfiedBy(argument); }
            catch { return false; }
        }

        public Type ForType { get { return _forType; } }
        public Action<object> Action { get; set; }
        public override string ToString() { return _matcher.ToString(); }

        public IArgumentSpecification CreateCopyMatchingAnyArgOfType(Type requiredType)
        {
            return new ArgumentSpecification(requiredType, new AnyArgumentMatcher(requiredType)) { Action = RunActionIfTypeIsCompatible };
        }

        private void RunActionIfTypeIsCompatible(object argument)
        {
            if (!argument.IsCompatibleWith(ForType)) return;
            Action(argument);
        }
    }
}