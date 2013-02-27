using System;

namespace NSubstitute.Core
{
    public class ConfigureCall : IConfigureCall
    {
        private readonly ICallResults _configuredResults;
        private readonly ICallActions _callActions;
        private readonly IGetCallSpec _getCallSpec;

        public ConfigureCall(ICallResults configuredResults, ICallActions callActions, IGetCallSpec getCallSpec)
        {
            _configuredResults = configuredResults;
            _callActions = callActions;
            _getCallSpec = getCallSpec;
        }

        public ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs)
        {
            var spec = _getCallSpec.FromLastCall(matchArgs);
            _configuredResults.SetResult(spec, valueToReturn);
            return new ConfiguredCall(action => _callActions.Add(spec, action));
        }

        public void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs)
        {
            var callSpecification = _getCallSpec.FromCall(call, matchArgs);
            _configuredResults.SetResult(callSpecification, valueToReturn);
        }
    }
}