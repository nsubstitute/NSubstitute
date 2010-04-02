using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace NSubstitute
{
    public class ArgumentsEqualMatcher : ICallMatcher
    {
        private readonly IArgumentEqualityComparer _equalityComparer;

        public ArgumentsEqualMatcher(IArgumentEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool IsMatch(ICall call, ICallSpecification callSpecification)
        {
            var callArguments = call.GetArguments();
            var argumentMatchers = callSpecification.ArgumentSpecifications;
            if (DifferentNumberOfArguments(callArguments, argumentMatchers)) return false;
            var numberOfArguments = callArguments.Length;

            return Enumerable.Range(0, numberOfArguments).All(index => argumentMatchers[index].IsSatisfiedBy(callArguments[index]));
        }

        private bool AreArgumentsEqual(object firstCallArgument, object secondCallArgument)
        {
            return _equalityComparer.Equals(firstCallArgument, secondCallArgument);
        }

        private bool DifferentNumberOfArguments(object[] firstCallArguments, object[] secondCallArguments)
        {
            return firstCallArguments.Length != secondCallArguments.Length;
        }

        private bool DifferentNumberOfArguments(ICollection<object> callArguments, ICollection<IArgumentSpecification> argumentMatchers)
        {
            return callArguments.Count != argumentMatchers.Count;
        }
    }
}