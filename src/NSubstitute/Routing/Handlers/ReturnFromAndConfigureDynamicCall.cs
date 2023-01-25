using NSubstitute.Core;
using System;
using System.Runtime.CompilerServices;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnFromAndConfigureDynamicCall : ICallHandler
    {
        private static readonly Type DynamicAttributeType = typeof(DynamicAttribute);
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
            var returnParameter = call.GetMethodInfo().ReturnParameter;
            if (returnParameter == null)
            {
                return false;
            }

            bool isDynamic;
            isDynamic = returnParameter.GetCustomAttributes(DynamicAttributeType, inherit: false).Length != 0;
            return isDynamic;
        }

        public class DynamicStub
        {
            public ConfiguredCall Returns<T>(T? returnThis, params T?[] returnThese)
            {
                return default(T).Returns(returnThis, returnThese);
            }

            public ConfiguredCall Returns<T>(Func<CallInfo, T?> returnThis, params Func<CallInfo, T?>[] returnThese)
            {
                return default(T).Returns(returnThis, returnThese);
            }

            public ConfiguredCall ReturnsForAnyArgs<T>(T? returnThis, params T?[] returnThese)
            {
                return default(T).ReturnsForAnyArgs(returnThis, returnThese);
            }

            public ConfiguredCall ReturnsForAnyArgs<T>(Func<CallInfo, T?> returnThis, params Func<CallInfo, T?>[] returnThese)
            {
                return default(T).ReturnsForAnyArgs(returnThis, returnThese);
            }
        }
    }
}