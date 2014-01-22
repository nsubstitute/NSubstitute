#if NET4
using System;
using System.Linq;
using System.Runtime.CompilerServices;

using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoDynamicProvider : IAutoValueProvider
    {
        private readonly IReceivedCalls _receivedCalls;

        public AutoDynamicProvider(IReceivedCalls receivedCalls)
        {
            _receivedCalls = receivedCalls;
        }

        public bool CanProvideValueFor(Type type)
        {
            var lastCall = _receivedCalls.AllCalls().FirstOrDefault();
            if (lastCall == null) return false;
            var returnParameter = lastCall.GetMethodInfo().ReturnParameter;
            if (returnParameter == null) return false;
            var isDynamic = returnParameter.GetCustomAttributes(typeof(DynamicAttribute), false).Any();
            return isDynamic;
        }

        public object GetValue(Type type)
        {
            return new DynamicStub();
        }

        public class DynamicStub
        {
            public ConfiguredCall Returns<T>(T returnThis, params T[] returnThese)
            {
                return SubstituteExtensions.Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
            }

            public ConfiguredCall Returns<T>(Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
            {
                return SubstituteExtensions.Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
            }

            public ConfiguredCall ReturnsForAnyArgs<T>(T returnThis, params T[] returnThese)
            {
                return SubstituteExtensions.Returns(MatchArgs.Any, returnThis, returnThese);
            }

            public ConfiguredCall ReturnsForAnyArgs<T>(Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
            {
                return SubstituteExtensions.Returns(MatchArgs.Any, returnThis, returnThese);
            }
        }
    }
}
#endif