using NSubstitute.Exceptions;

namespace NSubstitute.Core;

public class ConfigureCall(ICallResults configuredResults, ICallActions callActions, IGetCallSpec getCallSpec) : IConfigureCall
{
    public ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
    {
        var spec = getCallSpec.FromPendingSpecification(matchArgs, pendingSpecInfo);
        CheckResultIsCompatibleWithCall(valueToReturn, spec);
        configuredResults.SetResult(spec, valueToReturn);
        return new ConfiguredCall(action => callActions.Add(spec, action));
    }

    public void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs)
    {
        var spec = getCallSpec.FromCall(call, matchArgs);
        CheckResultIsCompatibleWithCall(valueToReturn, spec);
        configuredResults.SetResult(spec, valueToReturn);
    }

    private static void CheckResultIsCompatibleWithCall(IReturn valueToReturn, ICallSpecification spec)
    {
        var requiredReturnType = spec.ReturnType();
        if (!valueToReturn.CanBeAssignedTo(requiredReturnType))
        {
            throw new CouldNotSetReturnDueToTypeMismatchException(valueToReturn.TypeOrNull(), spec.GetMethodInfo());
        }
    }
}