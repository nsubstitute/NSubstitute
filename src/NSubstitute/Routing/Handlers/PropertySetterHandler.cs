using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class PropertySetterHandler : ICallHandler
    {
        private readonly IPropertyHelper _propertyHelper;
        private readonly IConfigureCall _configureCall;

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
                // It's important to use original arguments, as it provides better performance.
                // It's safe to use original arguments here, as only by-ref arguments might be modified,
                // which should never happen for this case.
                var valueBeingSetOnProperty = call.GetOriginalArguments().Last();
                _configureCall.SetResultForCall(callToPropertyGetter, new ReturnValue(valueBeingSetOnProperty), MatchArgs.AsSpecifiedInCall);
            }

            return RouteAction.Continue();
        }
    }
}