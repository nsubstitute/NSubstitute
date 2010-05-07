using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        private readonly ISubstitutionContext _context;

        public CallSpecificationFactory(ISubstitutionContext context)
        {
            _context = context;
        }

        public ICallSpecification CreateFrom(ICall call)
        {
            var result = new CallSpecification(call.GetMethodInfo());
            var argumentSpecs = _context.DequeueAllArgumentSpecifications();
            var arguments = call.GetArguments();
            if (argumentSpecs.Count == 0)
            {
                AddArgumentSpecsToCallSpec(result, arguments.Select(x => (IArgumentSpecification) new ArgumentEqualsSpecification(x)));
            }
            else if (argumentSpecs.Count == arguments.Length)
            {
                AddArgumentSpecsToCallSpec(result, argumentSpecs);
            }
            else
            {
                throw new AmbiguousArgumentsException(
                    "Cannot determine argument specifications to use. Please use specifications for all arguments.");
            }
            return result;
        }

        private void AddArgumentSpecsToCallSpec(ICallSpecification callSpec, IEnumerable<IArgumentSpecification> argSpecs)
        {
            foreach (var spec in argSpecs)
            {
                callSpec.ArgumentSpecifications.Add(spec);
            }   
        }
    }
}