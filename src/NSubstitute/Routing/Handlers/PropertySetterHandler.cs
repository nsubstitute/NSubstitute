using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class PropertySetterHandler : ICallHandler
    {
        private readonly IPropertyHelper _propertyHelper;
        readonly IConfigureCall ConfigureCall;

        public PropertySetterHandler(IPropertyHelper propertyHelper, IConfigureCall configureCall)
        {
            _propertyHelper = propertyHelper;
            ConfigureCall = configureCall;
        }

        public RouteAction Handle(ICall call)
        {
            if (_propertyHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _propertyHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().Last();
                ConfigureCall.SetResultForCall(callToPropertyGetter, new ReturnValue(valueBeingSetOnProperty), MatchArgs.AsSpecifiedInCall);
            }
            return RouteAction.Continue();
        }
    }
}