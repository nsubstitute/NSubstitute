using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallActions
    {
        void Add(ICallSpecification callSpecification, Action<object[]> action);
        IEnumerable<Action<object[]>> MatchingActions(ICall call);
    }
}