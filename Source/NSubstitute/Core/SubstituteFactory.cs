using System;

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

        public T Create<T>(Type[] additionalInterfaces, object[] constructorArguments) where T : class
        {
            var callRouter = _callRouterFactory.Create(_context);
            var proxy = _proxyFactory.GenerateProxy<T>(callRouter);
            _callRouterResolver.Register(proxy, callRouter);
            return proxy;
        }

        public ICallRouter GetCallRouterCreatedFor(object substitute)
        {
            return _callRouterResolver.ResolveFor(substitute);
        }
    }
}