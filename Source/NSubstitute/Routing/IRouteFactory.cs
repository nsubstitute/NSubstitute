﻿using System;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public interface IRouteFactory
    {
        IRoute CallBase(ISubstituteState state, MatchArgs matchArgs);
        IRoute CallQuery(ISubstituteState state);
        IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity);
        IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs);
        IRoute RaiseEvent(ISubstituteState state, Func<ICall, object[]> getEventArguments);
        IRoute RecordCallSpecification(ISubstituteState state);
        IRoute RecordReplay(ISubstituteState state);
    }
}