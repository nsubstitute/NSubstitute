using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class CallActions : ICallActions
    {
        readonly IList<CallAction> _actions = new List<CallAction>();

        public void Add(ICallSpecification callSpecification, Action<object[]> action)
        {
            _actions.Add(new CallAction(callSpecification, action));
        }

        public IEnumerable<Action<object[]>> MatchingActions(ICall call)
        {
            return _actions
                .Where(x => x.CallSpecification.IsSatisfiedBy(call))
                .Select(x => x.Action);
        }

        class CallAction
        {
            public CallAction(ICallSpecification callSpecification, Action<object[]> action)
            {
                CallSpecification = callSpecification;
                Action = action;
            }

            public ICallSpecification CallSpecification;
            public Action<object[]> Action;
        }
    }
}