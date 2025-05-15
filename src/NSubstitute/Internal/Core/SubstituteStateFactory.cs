using NSubstitute.Core;
using NSubstitute.Internal.Routing.AutoValues;

namespace NSubstitute.Internal.Core;

public class SubstituteStateFactory(ICallSpecificationFactory callSpecificationFactory,
    ICallInfoFactory callInfoFactory,
    IAutoValueProvidersFactory autoValueProvidersFactory) : ISubstituteStateFactory
{
    public ISubstituteState Create(ISubstituteFactory substituteFactory)
    {
        var autoValueProviders = autoValueProvidersFactory.CreateProviders(substituteFactory);
        return new SubstituteState(callSpecificationFactory, callInfoFactory, autoValueProviders);
    }
}