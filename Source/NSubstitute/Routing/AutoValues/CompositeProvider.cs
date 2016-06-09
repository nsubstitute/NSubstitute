using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class CompositeProvider : IAutoValueProvider
    {
        private readonly IEnumerable<IAutoValueProvider> _providers;

        public CompositeProvider(IEnumerable<IAutoValueProvider> providers)
        {
            if (providers == null)
                throw new ArgumentNullException("providers");

            _providers = providers;
        }

        public Maybe<object> GetValue(Type type)
        {
            return _providers
                .Aggregate(
                    Maybe.Nothing<object>(),
                    (x, p) => x.OrElse(p.GetValue(type)));
        }
    }
}