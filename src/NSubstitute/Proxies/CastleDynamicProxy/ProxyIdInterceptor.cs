using System;
using System.Globalization;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class ProxyIdInterceptor : IInterceptor
    {
        private readonly Type _primaryProxyType;
        private string? _cachedProxyId;

        public ProxyIdInterceptor(Type primaryProxyType)
        {
            _primaryProxyType = primaryProxyType;
        }

        public void Intercept(IInvocation invocation)
        {
            if (IsDefaultToStringMethod(invocation.Method))
            {
                invocation.ReturnValue = _cachedProxyId ??= GenerateId(invocation);
                return;
            }

            invocation.Proceed();
        }

        private string GenerateId(IInvocation invocation)
        {
            var proxy = invocation.InvocationTarget;

            var shortTypeName = _primaryProxyType.GetNonMangledTypeName();
            var proxyHashCode = proxy.GetHashCode();

            return string.Format(CultureInfo.InvariantCulture, "Substitute.{0}|{1:x8}", shortTypeName, proxyHashCode);
        }

        public static bool IsDefaultToStringMethod(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object)
                   && string.Equals(methodInfo.Name, nameof(ToString), StringComparison.Ordinal)
                   && methodInfo.GetParameters().Length == 0;
        }
    }
}