using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        private readonly ISubstitutionContext _context;
        readonly IArgumentSpecificationFactory _argumentSpecificationFactory;

        public CallSpecificationFactory(ISubstitutionContext context, IArgumentSpecificationFactory argumentSpecificationFactory)
        {
            _context = context;
            _argumentSpecificationFactory = argumentSpecificationFactory;
        }

        public ICallSpecification CreateFrom(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            var result = new CallSpecification(methodInfo);
            var argumentSpecs = _context.DequeueAllArgumentSpecifications();
            var arguments = call.GetArguments();
            var parameterInfos = methodInfo.GetParameters();
            var argumentSpecificationsForCall = _argumentSpecificationFactory.Create(argumentSpecs, arguments, parameterInfos);
            AddArgumentSpecsToCallSpec(result, argumentSpecificationsForCall);
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