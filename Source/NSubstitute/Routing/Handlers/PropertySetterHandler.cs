using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class PropertySetterHandler : ICallHandler
    {
        private readonly IPropertyHelper _propertyHelper;
        readonly IConfigureCall _configureCall;

        public PropertySetterHandler(IPropertyHelper propertyHelper, IConfigureCall configureCall)
        {
            _propertyHelper = propertyHelper;
            _configureCall = configureCall;
        }

        public RouteAction Handle(ICall call)
        {
            if (_propertyHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _propertyHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().Last();
                _configureCall.SetResultForCall(callToPropertyGetter, new ReturnValue(valueBeingSetOnProperty), MatchArgs.AsSpecifiedInCall);
            }
            return RouteAction.Continue();
        }
    }
}