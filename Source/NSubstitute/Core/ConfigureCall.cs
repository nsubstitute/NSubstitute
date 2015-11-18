namespace NSubstitute.Core
{
    public class ConfigureCall : IConfigureCall
    {
        private readonly ICallResults _configuredResults;
        private readonly ICallActions _callActions;
        private readonly IGetCallSpec _getCallSpec;
        private readonly IConfigureCallValidator _configureValidator;

        public ConfigureCall(ICallResults configuredResults, ICallActions callActions, IGetCallSpec getCallSpec)
            : this(configuredResults, callActions, getCallSpec, new ConfigureCallValidator(configuredResults))
        {
        }

        internal ConfigureCall(ICallResults configuredResults, ICallActions callActions, IGetCallSpec getCallSpec, IConfigureCallValidator configureValidator)
        {
            _configuredResults = configuredResults;
            _callActions = callActions;
            _getCallSpec = getCallSpec;
            _configureValidator = configureValidator;
        }

        public ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs)
        {
            var spec = _getCallSpec.FromLastCall(matchArgs);
            _configureValidator.CheckResultIsCompatibleWithCall(valueToReturn, spec);
            _configureValidator.CheckForSpecOverlap(spec);
            _configuredResults.SetResult(spec, valueToReturn);
            return new ConfiguredCall(action => _callActions.Add(spec, action));
        }

        public void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs)
        {
            var spec = _getCallSpec.FromCall(call, matchArgs);
            _configureValidator.CheckResultIsCompatibleWithCall(valueToReturn, spec);
            _configuredResults.SetResult(spec, valueToReturn);
        }
    }
}