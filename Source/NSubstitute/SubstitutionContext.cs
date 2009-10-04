using NSubstitute.Proxies.CastleDynamicProxy;

namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }
        IInvocationHandler _lastInvocationHandler;
        ISubstituteFactory _substituteFactory;

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            var invocationHandlerFactory = new InvocationHandlerFactory();
            var proxyGenerator = new CastleProxyGeneratorWrapper();
            var interceptorFactory = new CastleInterceptorFactory();
            var proxyFactory = new CastleDynamicProxyFactory(proxyGenerator, interceptorFactory);
            _substituteFactory = new SubstituteFactory(this, invocationHandlerFactory, proxyFactory);
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public void LastInvocationShouldReturn<T>(T value)
        {            
            if (_lastInvocationHandler == null) throw new SubstituteException();
            _lastInvocationHandler.LastInvocationShouldReturn(value);
        }

        public void LastInvocationHandlerInvoked(IInvocationHandler _invocationHandler)
        {
            _lastInvocationHandler = _invocationHandler;
        }

        public ISubstituteFactory GetSubstituteFactory()
        {
            return _substituteFactory;
        }
    }
}