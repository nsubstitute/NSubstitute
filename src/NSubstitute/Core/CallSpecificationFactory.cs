using NSubstitute.Core.Arguments;

namespace NSubstitute.Core;

public class CallSpecificationFactory(IArgumentSpecificationsFactory argumentSpecificationsFactory) : ICallSpecificationFactory
{
    public ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs)
    {
        var methodInfo = call.GetMethodInfo();
        var argumentSpecs = call.GetArgumentSpecifications();
        var arguments = call.GetOriginalArguments();
        var parameterInfos = call.GetParameterInfos();
        var argumentSpecificationsForCall = argumentSpecificationsFactory.Create(argumentSpecs, arguments, parameterInfos, methodInfo, matchArgs);
        return new CallSpecification(methodInfo, argumentSpecificationsForCall);
    }
}
