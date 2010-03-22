using System;

namespace NSubstitute
{
    public class ArgumentMatcher : IArgumentMatcher
    {
        private Predicate<object> _matchingCriteria;

        public ArgumentMatcher(Predicate<object> matchingCriteria)
        {
            _matchingCriteria = matchingCriteria;
        }

        public bool Matches(object argument)
        {
            return _matchingCriteria(argument);
        }
    }
}