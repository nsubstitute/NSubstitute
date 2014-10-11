using System;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly ICallRouterFactory _callRouterFactory;
        readonly IProxyFactory _proxyFactory;
        readonly ICallRouterResolver _callRouterResolver;
        readonly IMixinFactory _mixinFactory;

        public SubstituteFactory(ISubstitutionContext context, ICallRouterFactory callRouterFactory, IProxyFactory proxyFactory, ICallRouterResolver callRouterResolver, IMixinFactory mixinFactory)
        {
            _context = context;
            _callRouterFactory = callRouterFactory;
            _proxyFactory = proxyFactory;
            _callRouterResolver = callRouterResolver;
            _mixinFactory = mixinFactory;
        }

        /// <summary>
        /// Create a substitute for the given types.
        /// </summary>
        /// <param name="typesToProxy"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public object Create(Type[] typesToProxy, object[] constructorArguments)
        {
            return Create(typesToProxy, constructorArguments, SubstituteConfig.OverrideAllCalls);
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
            if (primaryProxyType.IsSubclassOf(typeof (Delegate)) || !primaryProxyType.IsClass)
            {
                throw new CanNotPartiallySubForInterfaceOrDelegateException(primaryProxyType);
            }
            return Create(typesToProxy, constructorArguments, SubstituteConfig.CallBaseByDefault);
        }

        private object Create(Type[] typesToProxy, object[] constructorArguments, SubstituteConfig config)  
        {
			var substituteState = new SubstituteState(_context, config);

            var callRouter = _callRouterFactory.Create(_context, substituteState);
            var primaryProxyType = GetPrimaryProxyType(typesToProxy);
            var additionalTypes = typesToProxy.Where(x => x != primaryProxyType).ToArray();
            var mixins = _mixinFactory.Create(primaryProxyType, additionalTypes, _context, substituteState);

            var proxy = _proxyFactory.GenerateProxy(callRouter, primaryProxyType, additionalTypes, constructorArguments, mixins);
            _callRouterResolver.Register(proxy, callRouter);
            return proxy;
        }

        private Type GetPrimaryProxyType(Type[] typesToProxy)
        {
            if (typesToProxy.Any(x => x.IsSubclassOf(typeof(Delegate)))) return typesToProxy.First(x => x.IsSubclassOf(typeof(Delegate)));
            if (typesToProxy.Any(x => x.IsClass)) return typesToProxy.First(x => x.IsClass);
            return typesToProxy.First();
        }

        public ICallRouter GetCallRouterCreatedFor(object substitute)
        {
            return _callRouterResolver.ResolveFor(substitute);
        }
    }
}