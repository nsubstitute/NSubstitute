using System;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public class SubstituteStateFactory : ISubstituteStateFactory
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallInfoFactory _callInfoFactory;
        private readonly IAutoValueProvidersFactory _autoValueProvidersFactory;

        public SubstituteStateFactory(ICallSpecificationFactory callSpecificationFactory,
            ICallInfoFactory callInfoFactory,
            IAutoValueProvidersFactory autoValueProvidersFactory)
        {
            _callSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
            _callInfoFactory = callInfoFactory ?? throw new ArgumentNullException(nameof(callInfoFactory));
            _autoValueProvidersFactory = autoValueProvidersFactory ?? throw new ArgumentNullException(nameof(autoValueProvidersFactory));
        }

        public ISubstituteState Create(ISubstituteFactory substituteFactory)
        {
            var autoValueProviders = _autoValueProvidersFactory.CreateProviders(substituteFactory);
            return new SubstituteState(_callSpecificationFactory, _callInfoFactory, autoValueProviders);
        }
    }
}