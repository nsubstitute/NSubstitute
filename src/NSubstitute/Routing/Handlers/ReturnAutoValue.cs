using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Routing.Handlers;

public enum AutoValueBehaviour
{
    UseValueForSubsequentCalls,
    ReturnAndForgetValue
}
public class ReturnAutoValue(AutoValueBehaviour autoValueBehaviour, IEnumerable<IAutoValueProvider> autoValueProviders, ICallResults callResults, ICallSpecificationFactory callSpecificationFactory) : ICallHandler
{
    private readonly IAutoValueProvider[] _autoValueProviders = autoValueProviders.AsArray();

    public RouteAction Handle(ICall call)
    {
        if (callResults.TryGetResult(call, out var cachedResult))
        {
            return RouteAction.Return(cachedResult);
        }

        var type = call.GetReturnType();

        // This is a hot method which is invoked frequently and has major impact on performance.
        // Therefore, the LINQ cycle was unwinded to loop.
        foreach (var autoValueProvider in _autoValueProviders)
        {
            if (autoValueProvider.CanProvideValueFor(type))
            {
                return RouteAction.Return(GetResultValueUsingProvider(call, type, autoValueProvider));
            }
        }

        return RouteAction.Continue();
    }

    private object? GetResultValueUsingProvider(ICall call, Type type, IAutoValueProvider provider)
    {
        var valueToReturn = provider.GetValue(type);
        if (autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
        {
            var spec = callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
            callResults.SetResult(spec, new ReturnValue(valueToReturn));
        }

        return valueToReturn;
    }
}