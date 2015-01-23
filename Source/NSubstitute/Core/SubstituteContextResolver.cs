using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class SubstituteContextResolver : ISubstituteContextResolver
    {
        private readonly IDictionary<object, ISubstituteContext> _substituteContextMappings = new Dictionary<object, ISubstituteContext>();

        public ISubstituteContext ResolveFor(object substitute)
        {
            if (substitute == null) throw new ArgumentNullException();

            ISubstituteContext substituteContext;
            if (!_substituteContextMappings.TryGetValue(substitute, out substituteContext))
            {
                substituteContext = new SubstituteContext();
                _substituteContextMappings.Add(substitute, substituteContext);
            }

            return substituteContext;
        }
    }
}