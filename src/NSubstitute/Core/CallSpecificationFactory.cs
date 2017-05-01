using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        readonly IArgumentSpecificationsFactory _argumentSpecificationsFactory;

        public CallSpecificationFactory(IArgumentSpecificationsFactory argumentSpecificationsFactory)
        {
            _argumentSpecificationsFactory = argumentSpecificationsFactory;
        }

        public ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs)
        {
            var methodInfo = call.GetMethodInfo();
            var argumentSpecs = call.GetArgumentSpecifications();
            var arguments = call.GetOriginalArguments();
            var parameterInfos = call.GetParameterInfos();
            var argumentSpecificationsForCall = _argumentSpecificationsFactory.Create(argumentSpecs, arguments, parameterInfos, matchArgs);
            return new CallSpecification(methodInfo, argumentSpecificationsForCall);
        }
    }
}
