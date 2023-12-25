using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues;

public interface IAutoValueProvidersFactory
{
    IReadOnlyCollection<IAutoValueProvider> CreateProviders(ISubstituteFactory substituteFactory);
}