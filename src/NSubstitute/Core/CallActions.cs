using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallActions : ICallActions
    {
        static readonly Action<CallInfo> EmptyAction = x => { };
        readonly ICallInfoFactory _callInfoFactory;
        readonly List<CallAction> _actions = new List<CallAction>();

        public CallActions(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
        }

        public void Add(ICallSpecification callSpecification, Action<CallInfo> action)
        {
            _actions.Add(new CallAction(callSpecification, action));
        }

        public void Add(ICallSpecification callSpecification)
        {
            _actions.Add(new CallAction(callSpecification, EmptyAction));
        }

        public void MoveActionsForSpecToNewSpec(ICallSpecification oldCallSpecification, ICallSpecification newCallSpecification)
        {
            foreach (var action in _actions.Where(x => x.IsFor(oldCallSpecification)))
            {
                action.UpdateCallSpecification(newCallSpecification);
            }
        }

	    public void Clear()
	    {
		    _actions.Clear();
	    }

	    public void InvokeMatchingActions(ICall call)
        {
            var callInfo = _callInfoFactory.Create(call);
            foreach (var action in _actions.Where(x => x.IsSatisfiedBy(call)))
            {
                action.Invoke(callInfo);
            }
        }

        class CallAction
        {
            public CallAction(ICallSpecification callSpecification, Action<CallInfo> action)
            {
                _callSpecification = callSpecification;
                _action = action;
            }

            private ICallSpecification _callSpecification;
            private readonly Action<CallInfo> _action;
            public bool IsSatisfiedBy(ICall call) { return _callSpecification.IsSatisfiedBy(call); }
            public void Invoke(CallInfo callInfo)
            {
                _action(callInfo);
                _callSpecification.InvokePerArgumentActions(callInfo);
            }
            public bool IsFor(ICallSpecification spec) { return _callSpecification == spec; }
            public void UpdateCallSpecification(ICallSpecification spec) { _callSpecification = spec; }
        }
    }
}