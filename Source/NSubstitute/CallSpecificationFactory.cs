using System.Collections.Generic;
using NSubstitute.Exceptions;

namespace NSubstitute
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        public ICallSpecification Create(ICall call, IList<IArgumentMatcher> argumentMatchers)
        {
            var result = new CallSpecification(call.GetMethodInfo());

            var arguments = call.GetArguments();
            if (argumentMatchers.Count == 0)
            {
                foreach (var argument in arguments)
                {
                    var specifiedArgument = argument;
                    result.ArgumentMatchers.Add(new ArgumentMatcher(o => EqualityComparer<object>.Default.Equals(o, specifiedArgument)));
                }
            }
            else if (arguments.Length == argumentMatchers.Count)
            {
                foreach (var argumentMatcher in argumentMatchers)
                {
                    result.ArgumentMatchers.Add(argumentMatcher);
                }
            }
            else
            {
                throw new AmbiguousParametersException("Cannot determine parameter specifications to use. Please use specifications for all arguments.");
            }

            return result;
        }
    }
}