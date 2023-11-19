using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.CastleDynamicProxy;

namespace NSubstitute.Proxies.DelegateProxy
{
    [Obsolete("This class is deprecated and will be removed in future versions of the product.")]
    public class DelegateProxyFactory : IProxyFactory
    {
        private readonly CastleDynamicProxyFactory _castleObjectProxyFactory;

        public DelegateProxyFactory(CastleDynamicProxyFactory objectProxyFactory)
        {
            _castleObjectProxyFactory = objectProxyFactory ?? throw new ArgumentNullException(nameof(objectProxyFactory));
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, object?[]? constructorArguments)
        {
            // Castle factory can now resolve delegate proxies as well.
            return _castleObjectProxyFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments);
        }
    }
}