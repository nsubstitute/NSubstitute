using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.CastleDynamicProxy;

namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }
        ICallHandler _lastCallHandler;
        ISubstituteFactory _substituteFactory;
        IList<IArgumentSpecification> _argumentMatchers;

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            var callHandlerFactory = new CallHandlerFactory();
            var interceptorFactory = new CastleInterceptorFactory();
            var proxyFactory = new CastleDynamicProxyFactory(new ProxyGenerator(), interceptorFactory);
            _substituteFactory = new SubstituteFactory(this, callHandlerFactory, proxyFactory);
            _argumentMatchers = new List<IArgumentSpecification>();
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public void LastCallShouldReturn<T>(T value)
        {            
            if (_lastCallHandler == null) throw new SubstituteException();
            _lastCallHandler.LastCallShouldReturn(value);
        }

        public void LastCallHandler(ICallHandler callHandler)
        {
            _lastCallHandler = callHandler;
        }

        public ISubstituteFactory GetSubstituteFactory()
        {
            return _substituteFactory;
        }

        public ICallHandler GetCallHandlerFor(object substitute)
        {
            var isHandler = substitute is ICallHandler;
            if (!isHandler) throw new NotASubstituteException();
            return (ICallHandler) substitute;
        }

        public void EnqueueArgumentSpecification(IArgumentSpecification spec)
        {
            _argumentMatchers.Add(spec);
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecifications()
        {
            var result = _argumentMatchers;
            _argumentMatchers = new List<IArgumentSpecification>();
            return result;
        }
    }
}