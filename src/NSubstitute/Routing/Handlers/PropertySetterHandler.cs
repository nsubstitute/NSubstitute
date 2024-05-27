using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class PropertySetterHandler(IPropertyHelper propertyHelper, IConfigureCall configureCall) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (propertyHelper.IsCallToSetAReadWriteProperty(call))
        {
            var callToPropertyGetter = propertyHelper.CreateCallToPropertyGetterFromSetterCall(call);
            // It's important to use original arguments, as it provides better performance.
            // It's safe to use original arguments here, as only by-ref arguments might be modified,
            // which should never happen for this case.
            var valueBeingSetOnProperty = call.GetOriginalArguments().Last();
            configureCall.SetResultForCall(callToPropertyGetter, new ReturnValue(valueBeingSetOnProperty), MatchArgs.AsSpecifiedInCall);
        }

        return RouteAction.Continue();
    }
}