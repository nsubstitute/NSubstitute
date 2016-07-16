using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Proxies;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }

        readonly ISubstituteFactory _substituteFactory;
        readonly SequenceNumberGenerator _sequenceNumberGenerator = new SequenceNumberGenerator();
        readonly RobustThreadLocal<ICallRouter> _lastCallRouter = new RobustThreadLocal<ICallRouter>();
        readonly RobustThreadLocal<IList<IArgumentSpecification>> _argumentSpecifications = new RobustThreadLocal<IList<IArgumentSpecification>>(() => new List<IArgumentSpecification>());
        readonly RobustThreadLocal<Func<ICall, object[]>> _getArgumentsForRaisingEvent = new RobustThreadLocal<Func<ICall, object[]>>();
        readonly RobustThreadLocal<Query> _currentQuery = new RobustThreadLocal<Query>();

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            var callRouterFactory = new CallRouterFactory();
            var dynamicProxyFactory = new CastleDynamicProxyFactory();
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
        public SequenceNumberGenerator SequenceNumberGenerator { get { return _sequenceNumberGenerator; } }
        public bool IsQuerying { get { return _currentQuery.Value != null; } }

        public ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs)
        {
            if (_lastCallRouter.Value == null) throw new CouldNotSetReturnDueToNoLastCallException();
            if (_argumentSpecifications.Value.Any())
            {
                //Clear invalid arg specs so they will not affect other tests
                _argumentSpecifications.Value.Clear();
                throw new UnexpectedArgumentMatcherException();
            }
            var configuredCall = _lastCallRouter.Value.LastCallShouldReturn(value, matchArgs);
            ClearLastCallRouter();
            return configuredCall;
        }

        public void ClearLastCallRouter()
        {
            _lastCallRouter.Value = null;
        }

        public IRouteFactory GetRouteFactory() { return new RouteFactory(); }

        public void LastCallRouter(ICallRouter callRouter)
        {
            _lastCallRouter.Value = callRouter;
            RaiseEventIfSet(callRouter);
        }

        void RaiseEventIfSet(ICallRouter callRouter)
        {
            if (_getArgumentsForRaisingEvent.Value != null)
            {
                var routes = new RouteFactory();
                callRouter.SetRoute(x => routes.RaiseEvent(x, _getArgumentsForRaisingEvent.Value));
                _getArgumentsForRaisingEvent.Value = null;
            }
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

        public void AddToQuery(object target, ICallSpecification callSpecification)
        {
            var query = _currentQuery.Value;
            if (query == null) { throw new NotRunningAQueryException(); }
            query.Add(callSpecification, target);
        }

        public IQueryResults RunQuery(Action calls)
        {
            var query = new Query();
            _currentQuery.Value = query;
            try
            {
                calls();
            }
            finally
            {
                _currentQuery.Value = null;
            }
            return query.Result();
        }
    }
}