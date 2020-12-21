using System;
using System.Collections.Concurrent;

namespace NSubstitute.Core
{
    public class CallActions : ICallActions
    {
        private static readonly Action<CallInfo> EmptyAction = x => { };

        private readonly ICallInfoFactory _callInfoFactory;
        // Collection consideration.
        // We need to have a thread-safe collection which should be enumerated in add order.
        // Even though Queue allocates on each enumeration, we expect callbacks to occur rarely,
        // so it shouldn't be a big issue.
        // If we want to optimize it later, we should probably take a look at System.Collections.Immutable.
        private ConcurrentQueue<CallAction> _actions = new();

        public CallActions(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
        }

        public void Add(ICallSpecification callSpecification, Action<CallInfo> action)
        {
            _actions.Enqueue(new CallAction(callSpecification, action));
        }

        public void Add(ICallSpecification callSpecification)
        {
            Add(callSpecification, EmptyAction);
        }

        public void MoveActionsForSpecToNewSpec(ICallSpecification oldCallSpecification, ICallSpecification newCallSpecification)
        {
            foreach (var action in _actions)
            {
                if (action.IsFor(oldCallSpecification))
                {
                    action.UpdateCallSpecification(newCallSpecification);
                }
            }
        }

        public void Clear()
        {
            // Collection doesn't have a clear method.
            _actions = new ConcurrentQueue<CallAction>();
        }

        public void InvokeMatchingActions(ICall call)
        {
            // Performance optimization - enumeration allocates enumerator object.
            if (_actions.IsEmpty)
            {
                return;
            }

            CallInfo? callInfo = null;
            foreach (var action in _actions)
            {
                if (!action.IsSatisfiedBy(call))
                    continue;

                // Optimization. Initialize call lazily, as most of times there are no callbacks.
                callInfo ??= _callInfoFactory.Create(call);

                action.Invoke(callInfo);
            }
        }

        private class CallAction
        {
            public CallAction(ICallSpecification callSpecification, Action<CallInfo> action)
            {
                _callSpecification = callSpecification;
                _action = action;
            }

            private ICallSpecification _callSpecification;
            private readonly Action<CallInfo> _action;
            public bool IsSatisfiedBy(ICall call) => _callSpecification.IsSatisfiedBy(call);

            public void Invoke(CallInfo callInfo)
            {
                _action(callInfo);
                _callSpecification.InvokePerArgumentActions(callInfo);
            }
            public bool IsFor(ICallSpecification spec) => _callSpecification == spec;
            public void UpdateCallSpecification(ICallSpecification spec) => _callSpecification = spec;
        }
    }
}
