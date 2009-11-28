using System.Collections;
using System.Linq;
namespace NSubstitute
{
    public class ArgumentsEqualMatcher : IInvocationMatcher
    {
        private readonly IArgumentEqualityComparer _equalityComparer;

        public ArgumentsEqualMatcher(IArgumentEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool IsMatch(IInvocation first, IInvocation second)
        {
            var firstCallArguments = first.GetArguments();
            var secondCallArguments = second.GetArguments();
            if (DifferentNumberOfArguments(firstCallArguments, secondCallArguments)) return false;
            var numberOfArguments = firstCallArguments.Length;
            return Enumerable.Range(0, numberOfArguments).All(index => AreArgumentsEqual(firstCallArguments[index], secondCallArguments[index]));
        }

        private bool AreArgumentsEqual(object firstCallArgument, object secondCallArgument)
        {
            return _equalityComparer.Equals(firstCallArgument, secondCallArgument);
        }

        private bool DifferentNumberOfArguments(object[] firstCallArguments, object[] secondCallArguments)
        {
            return firstCallArguments.Length != secondCallArguments.Length;
        }
    }
}