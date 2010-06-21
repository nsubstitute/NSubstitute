using System;
using System.Linq;

namespace NSubstitute.Core
{
    public class SubstituteFactory : ISubstituteFactory
    {
        readonly ISubstitutionContext _context;
        readonly ICallRouterFactory _callRouterFactory;
        readonly IProxyFactory _proxyFactory;
        readonly ICallRouterResolver _callRouterResolver;

        public SubstituteFactory(ISubstitutionContext context, ICallRouterFactory callRouterFactory, IProxyFactory proxyFactory, ICallRouterResolver callRouterResolver)
        {
            _context = context;
            _callRouterFactory = callRouterFactory;
            _proxyFactory = proxyFactory;
            _callRouterResolver = callRouterResolver;
        }

        public object Create(Type[] typesToProxy, object[] constructorArguments)  
        {
            var callRouter = _callRouterFactory.Create(_context);
            var primaryProxyType = GetPrimaryProxyType(typesToProxy);
            var additionalTypes = typesToProxy.Where(x => x != primaryProxyType).ToArray();
            var proxy = _proxyFactory.GenerateProxy(callRouter, primaryProxyType, additionalTypes, constructorArguments);
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