using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecification : IArgumentSpecification
    {
        private readonly Type _forType;
        private readonly IArgumentMatcher _matcher;

        public ArgumentSpecification(Type forType, IArgumentMatcher matcher)
        {
            _forType = forType;
            _matcher = matcher;
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
            var requiredType = (ForType.IsByRef) ? ForType.GetElementType() : ForType;
            return argument == null ? TypeCanBeNull(requiredType) : requiredType.IsAssignableFrom(argument.GetType());
        }

        private bool TypeCanBeNull(Type type) { return !type.IsValueType; }
    }
}