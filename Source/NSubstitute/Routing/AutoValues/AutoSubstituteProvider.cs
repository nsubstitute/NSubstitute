using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoSubstituteProvider : IAutoValueProvider
    {
        private readonly ISubstituteFactory _substituteFactory;

        public AutoSubstituteProvider(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public bool CanProvideValueFor(Type type)
        {
            return type.IsInterface || type.IsSubclassOf(typeof(Delegate));
        }

        public object GetValue(Type type)
        {
            return _substituteFactory.Create(new[] { type }, new object[0]);
        }
    }
}