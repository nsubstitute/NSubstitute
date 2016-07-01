using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromAndConfigureDynamicCall : ICallHandler
    {
        private readonly IConfigureCall _configureCall;

        public ReturnFromAndConfigureDynamicCall(IConfigureCall configureCall)
        {
            _configureCall = configureCall;
        }

        public RouteAction Handle(ICall call)
        {
            if (ReturnsDynamic(call))
            {
                var stubToReturn = new DynamicStub();
                _configureCall.SetResultForCall(call, new ReturnValue(stubToReturn), MatchArgs.AsSpecifiedInCall);
                return RouteAction.Return(new DynamicStub());
            }
            else
            {
                return RouteAction.Continue();
            }
        }

        private bool ReturnsDynamic(ICall call)
        {
#if (NET4 || NET45 || NETSTANDARD1_5)
            var returnParameter = call.GetMethodInfo().ReturnParameter;
            if (returnParameter == null) return false;
            var dynamicAttribute = typeof (System.Runtime.CompilerServices.DynamicAttribute);
            var customAttributes = returnParameter.GetCustomAttributes(dynamicAttribute, false);
            var isDynamic = customAttributes != null && customAttributes.Any();
            return isDynamic;
#else
            return false;
#endif
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