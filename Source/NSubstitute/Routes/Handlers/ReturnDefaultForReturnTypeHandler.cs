using System;
using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class ReturnDefaultForReturnTypeHandler : ICallHandler
    {
        public object Handle(ICall call)
        {
            var returnType = call.GetMethodInfo().ReturnType;
            if (IsVoid(returnType)) return null;
            if (returnType.IsValueType) return DefaultInstanceOfValueType(returnType);
            return null;
        }

        private bool IsVoid(Type returnType)
        {
            return returnType == typeof(void);
        }

        private object DefaultInstanceOfValueType(Type returnType)
        {
            return Activator.CreateInstance(returnType);
        }
    }
}