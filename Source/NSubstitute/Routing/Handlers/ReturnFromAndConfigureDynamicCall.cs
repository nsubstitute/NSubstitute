using System;
using System.Collections.Generic;
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
#if (NET4 || NET45)
            var methodInfo = call.GetMethodInfo();
            var returnParameter = methodInfo.ReturnParameter;
            if (returnParameter == null) return false;
            if (returnParameter.ParameterType.IsValueType) return false;

            var parameters = methodInfo.GetParameters();

            var allParameters = new[] { returnParameter }.Concat(parameters);
            return IsAnyOfParameterDynamic(allParameters);
#else
            return false;
#endif
        }

#if (NET4 || NET45)
        private bool IsAnyOfParameterDynamic(IEnumerable<ParameterInfo> customAttributeProviders)
        {
            var dynamicAttribute = typeof(System.Runtime.CompilerServices.DynamicAttribute);
            return customAttributeProviders.Any(x => x.GetCustomAttributes(dynamicAttribute, false).Any());
        }
#endif

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