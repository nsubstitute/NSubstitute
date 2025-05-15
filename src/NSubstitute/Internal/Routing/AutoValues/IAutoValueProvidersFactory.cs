using NSubstitute.Core;

namespace NSubstitute.Internal.Routing.AutoValues;

public interface IAutoValueProvidersFactory
{
    IReadOnlyCollection<IAutoValueProvider> CreateProviders(ISubstituteFactory substituteFactory);
}