using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        readonly Type _forType;
        readonly IArgumentMatcher _matcher;
        readonly IArgumentMatcher _compatibleMatcher;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher)
        {
            _forType = forType;
            _matcher = matcher;
            _compatibleMatcher = new AnyArgumentMatcher(forType);
            Action = x => { };
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (!ArgumentIsCompatibleWithType(argument)) return false;
            try { return _matcher.IsSatisfiedBy(argument); }
            catch { return false; }
        }

        public Type ForType { get { return _forType; } }
        public Action<object> Action { get; set; }
        public override string ToString() { return _matcher.ToString(); }

        private bool ArgumentIsCompatibleWithType(object argument) 
        {
            return _compatibleMatcher.IsSatisfiedBy(argument);
        }
    }
}