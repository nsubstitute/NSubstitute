using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class SubstituteFactory : ISubstituteFactory
    {
        private readonly ISubstituteStateFactory _substituteStateFactory;
        private readonly ICallRouterFactory _callRouterFactory;
        private readonly IProxyFactory _proxyFactory;

        public SubstituteFactory(ISubstituteStateFactory substituteStateFactory, ICallRouterFactory callRouterFactory, IProxyFactory proxyFactory)
        {
            _substituteStateFactory = substituteStateFactory ?? throw new ArgumentNullException(nameof(substituteStateFactory));
            _callRouterFactory = callRouterFactory ?? throw new ArgumentNullException(nameof(callRouterFactory));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        /// <summary>
        /// Create a substitute for the given types.
        /// </summary>
        /// <param name="typesToProxy"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public object Create(Type[] typesToProxy, object[] constructorArguments)
        {
            return Create(typesToProxy, constructorArguments, callBaseByDefault: false);
        }

        /// <summary>
        /// Create an instance of the given types, with calls configured to call the base implementation
        /// where possible. Parts of the instance can be substituted using 
        /// <see cref="SubstituteExtensions.Returns{T}(T,T,T[])">Returns()</see>.
        /// </summary>
        /// <param name="typesToProxy"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public object CreatePartial(Type[] typesToProxy, object[] constructorArguments)
        {
            var primaryProxyType = GetPrimaryProxyType(typesToProxy);
            if (primaryProxyType.GetTypeInfo().IsSubclassOf(typeof(Delegate)) || !primaryProxyType.GetTypeInfo().IsClass)
            {
                throw new CanNotPartiallySubForInterfaceOrDelegateException(primaryProxyType);
            }

            return Create(typesToProxy, constructorArguments, callBaseByDefault: true);
        }

        private object Create(Type[] typesToProxy, object[] constructorArguments, bool callBaseByDefault)
        {
            var substituteState = _substituteStateFactory.Create(this);
            substituteState.CallBaseConfiguration.CallBaseByDefault = callBaseByDefault;

            var callRouter = _callRouterFactory.Create(substituteState);
            var primaryProxyType = GetPrimaryProxyType(typesToProxy);
            var additionalTypes = typesToProxy.Where(x => x != primaryProxyType).ToArray();
            var proxy = _proxyFactory.GenerateProxy(callRouter, primaryProxyType, additionalTypes, constructorArguments);
            return proxy;
        }

        private Type GetPrimaryProxyType(Type[] typesToProxy)
        {
            return typesToProxy.FirstOrDefault(t => t.GetTypeInfo().IsSubclassOf(typeof(Delegate)))
                ?? typesToProxy.FirstOrDefault(t => t.GetTypeInfo().IsClass)
                ?? typesToProxy.First();
        }
    }
}