using System;
using System.Collections.Generic;
using System.Threading;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Proxies;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Routing.Definitions;

namespace NSubstitute.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        class RobustThreadLocal<T>
        {
            /* Delegates to ThreadLocal<T>, but wraps Value property access in try/catch to swallow ObjectDisposedExceptions.
             * These can occur if the Value property is access from the finalizer thread. Because we can't detect this, we'll
             * just swallow the exception (the finalizer thread won't be using any of these values anyway).
             */
            readonly ThreadLocal<T> _threadLocal;
            public RobustThreadLocal() { _threadLocal = new ThreadLocal<T>(); }
            public RobustThreadLocal(Func<T> initialValue) { _threadLocal = new ThreadLocal<T>(initialValue); }
            public T Value
            {
                get
                {
                    try { return _threadLocal.Value; }
                    catch (ObjectDisposedException) { return default(T); }
                }
                set 
                {
                    try { _threadLocal.Value = value; }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        public static ISubstitutionContext Current { get; set; }

        readonly ISubstituteFactory _substituteFactory;
        readonly RobustThreadLocal<ICallRouter> _lastCallRouter = new RobustThreadLocal<ICallRouter>();
        readonly RobustThreadLocal<IList<IArgumentSpecification>> _argumentSpecifications = new RobustThreadLocal<IList<IArgumentSpecification>>(() => new List<IArgumentSpecification>());
        readonly RobustThreadLocal<Func<ICall, object[]>> _getArgumentsForRaisingEvent = new RobustThreadLocal<Func<ICall, object[]>>();

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            var callRouterFactory = new CallRouterFactory();
            var interceptorFactory = new CastleInterceptorFactory();
            var dynamicProxyFactory = new CastleDynamicProxyFactory(interceptorFactory);
            var delegateFactory = new DelegateProxyFactory();
            var proxyFactory = new ProxyFactory(delegateFactory, dynamicProxyFactory);
            var callRouteResolver = new CallRouterResolver();
            _substituteFactory = new SubstituteFactory(this, callRouterFactory, proxyFactory, callRouteResolver);
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public ISubstituteFactory SubstituteFactory { get { return _substituteFactory; } }

        public void LastCallShouldReturn(IReturn value, MatchArgs matchArgs)
        {
            if (_lastCallRouter.Value == null) throw new CouldNotSetReturnException();
            _lastCallRouter.Value.LastCallShouldReturn(value, matchArgs);
            _lastCallRouter.Value = null;
        }

        public void LastCallRouter(ICallRouter callRouter)
        {
            _lastCallRouter.Value = callRouter;
            RaiseEventIfSet(callRouter);
        }

        void RaiseEventIfSet(ICallRouter callRouter)
        {
            if (_getArgumentsForRaisingEvent.Value != null)
            {
                callRouter.SetRoute<RaiseEventRoute>(_getArgumentsForRaisingEvent.Value);
                _getArgumentsForRaisingEvent.Value = null;
            }
        }

        public ISubstituteFactory GetSubstituteFactory()
        {
            return SubstituteFactory;
        }

        public ICallRouter GetCallRouterFor(object substitute)
        {
            return SubstituteFactory.GetCallRouterCreatedFor(substitute);
        }

        public void EnqueueArgumentSpecification(IArgumentSpecification spec)
        {
            _argumentSpecifications.Value.Add(spec);
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecifications()
        {
            var result = _argumentSpecifications.Value;
            _argumentSpecifications.Value = new List<IArgumentSpecification>();
            return result;
        }

        public void RaiseEventForNextCall(Func<ICall, object[]> getArguments)
        {
            _getArgumentsForRaisingEvent.Value = getArguments;
        }
    }
}