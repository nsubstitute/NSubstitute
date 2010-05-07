using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallActions
    {
        void Add(ICallSpecification callSpecification, Action<object[]> action);
        IEnumerable<Action<object[]>> MatchingActions(ICall call);
    }
}