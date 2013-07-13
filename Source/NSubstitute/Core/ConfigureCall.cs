using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ConfigureCall : IConfigureCall
    {
        private readonly ICallResults _configuredResults;
        private readonly ICallActions _callActions;
        private readonly IGetCallSpec _getCallSpec;
        private readonly ICallBaseSpecifications _callBaseSpecifications;

        public ConfigureCall(ICallResults configuredResults, ICallActions callActions, IGetCallSpec getCallSpec, ICallBaseSpecifications callBaseSpecifications)
        {
            _configuredResults = configuredResults;
            _callActions = callActions;
            _getCallSpec = getCallSpec;
            _callBaseSpecifications = callBaseSpecifications;
        }

        public ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs)
        {
            var spec = _getCallSpec.FromLastCall(matchArgs);
            CheckResultIsCompatibleWithCall(valueToReturn, spec);
            _configuredResults.SetResult(spec, valueToReturn);
            return new ConfiguredCall(action => _callActions.Add(spec, action));
        }

        public void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs)
        {
            var spec = _getCallSpec.FromCall(call, matchArgs);
            CheckResultIsCompatibleWithCall(valueToReturn, spec);
            _configuredResults.SetResult(spec, valueToReturn);
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
}