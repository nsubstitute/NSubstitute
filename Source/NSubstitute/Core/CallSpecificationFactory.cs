using System.Linq;
namespace NSubstitute.Core
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        readonly IArgumentSpecificationFactory _argumentSpecificationFactory;

        public CallSpecificationFactory(IArgumentSpecificationFactory argumentSpecificationFactory)
        {
            _argumentSpecificationFactory = argumentSpecificationFactory;
        }

        public ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs)
        {
            var methodInfo = call.GetMethodInfo();
            var argumentSpecs = call.GetArgumentSpecifications();
            var arguments = call.GetArguments();
            var parameterTypes = call.GetParameterTypes();
            var argumentSpecificationsForCall = _argumentSpecificationFactory.Create(argumentSpecs, arguments, parameterTypes, matchArgs);
            return new CallSpecification(methodInfo, argumentSpecificationsForCall);
        }
    }
}