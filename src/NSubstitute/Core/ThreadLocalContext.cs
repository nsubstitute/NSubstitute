using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ThreadLocalContext : IThreadLocalContext
    {
        private readonly RobustThreadLocal<ICallRouter> _lastCallRouter;
        private readonly RobustThreadLocal<IList<IArgumentSpecification>> _argumentSpecifications;
        private readonly RobustThreadLocal<Func<ICall, object[]>> _getArgumentsForRaisingEvent;
        private readonly RobustThreadLocal<IQuery> _currentQuery;
        private readonly RobustThreadLocal<PendingSpecificationInfo> _pendingSpecificationInfo;
        public IPendingSpecification PendingSpecification { get; }

        public ThreadLocalContext()
        {
            _lastCallRouter = new RobustThreadLocal<ICallRouter>();
            _argumentSpecifications = new RobustThreadLocal<IList<IArgumentSpecification>>(() => new List<IArgumentSpecification>());
            _getArgumentsForRaisingEvent = new RobustThreadLocal<Func<ICall, object[]>>();
            _currentQuery = new RobustThreadLocal<IQuery>();
            _pendingSpecificationInfo = new RobustThreadLocal<PendingSpecificationInfo>();

            PendingSpecification = new PendingSpecificationWrapper(_pendingSpecificationInfo);
        }

        public void SetLastCallRouter(ICallRouter callRouter)
        {
            _lastCallRouter.Value = callRouter;
        }

        public ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs)
        {
            var lastCallRouter = _lastCallRouter.Value;
            if (lastCallRouter == null)
                throw new CouldNotSetReturnDueToNoLastCallException();

            if(!PendingSpecification.HasPendingCallSpecInfo())
               throw new CouldNotSetReturnDueToMissingInfoAboutLastCallException();

            if (_argumentSpecifications.Value.Any())
            {
                // Clear invalid arg specs so they will not affect other tests.
                _argumentSpecifications.Value.Clear();
                throw new UnexpectedArgumentMatcherException();
            }

            var pendingSpecInfo = PendingSpecification.UseCallSpecInfo();
            var configuredCall = lastCallRouter.LastCallShouldReturn(value, matchArgs, pendingSpecInfo);
            ClearLastCallRouter();
            return configuredCall;
        }

        public void ClearLastCallRouter()
        {
            _lastCallRouter.Value = null;
        }

        public void EnqueueArgumentSpecification(IArgumentSpecification spec)
        {
            var queue = _argumentSpecifications.Value;
            if (queue == null)
                throw new SubstituteInternalException("Argument specification queue is null.");

            queue.Add(spec);
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecifications()
        {
            var queue = _argumentSpecifications.Value;
            if (queue == null)
                throw new SubstituteInternalException("Argument specification queue is null.");

            _argumentSpecifications.Value = new List<IArgumentSpecification>();
            return queue;
        }

        public void SetPendingRasingEventArgumentsFactory(Func<ICall, object[]> getArguments)
        {
            _getArgumentsForRaisingEvent.Value = getArguments;
        }

        public Func<ICall, object[]> UsePendingRaisingEventArgumentsFactory()
        {
            var result = _getArgumentsForRaisingEvent.Value;
            _getArgumentsForRaisingEvent.Value = null;
            return result;
        }

        public void RunInQueryContext(Action calls, IQuery query)
        {
            _currentQuery.Value = query;
            try
            {
                calls();
            }
            finally
            {
                _currentQuery.Value = null;
            }
        }

        public bool IsQuerying => _currentQuery.Value != null;

        public void RegisterInContextQuery(ICall call)
        {
            var query = _currentQuery.Value;
            if (query == null)
                throw new NotRunningAQueryException();

            query.RegisterCall(call);
        }

        private class PendingSpecificationWrapper : IPendingSpecification
        {
            private readonly RobustThreadLocal<PendingSpecificationInfo> _valueHolder;

            public PendingSpecificationWrapper(RobustThreadLocal<PendingSpecificationInfo> valueHolder)
            {
                _valueHolder = valueHolder;
            }

            public bool HasPendingCallSpecInfo()
            {
                return _valueHolder.Value != null;
            }

            public PendingSpecificationInfo UseCallSpecInfo()
            {
                var info = _valueHolder.Value;
                Clear();
                return info;
            }

            public void SetCallSpecification(ICallSpecification callSpecification)
            {
                _valueHolder.Value = PendingSpecificationInfo.FromCallSpecification(callSpecification);
            }

            public void SetLastCall(ICall lastCall)
            {
                _valueHolder.Value = PendingSpecificationInfo.FromLastCall(lastCall);
            }

            public void Clear()
            {
                _valueHolder.Value = null;
            }
        }
    }
}