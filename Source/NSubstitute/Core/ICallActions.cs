using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallActions
    {
        void Add(ICallSpecification callSpecification, Action<CallInfo> action);
        IEnumerable<Action<CallInfo>> MatchingActions(ICall call);
    }
}