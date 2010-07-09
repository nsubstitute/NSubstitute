using System.Linq;
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

        public ICallSpecification CreateFrom(ICall call, bool withAnyArguments)
        {
            var methodInfo = call.GetMethodInfo();
            var argumentSpecs = _context.DequeueAllArgumentSpecifications();
            var arguments = call.GetArguments();
            var parameterTypes = call.GetParameterTypes();
            var argumentSpecificationsForCall = _argumentSpecificationFactory.Create(argumentSpecs, arguments, parameterTypes, withAnyArguments);
            return new CallSpecification(methodInfo, argumentSpecificationsForCall);
        }
    }
}