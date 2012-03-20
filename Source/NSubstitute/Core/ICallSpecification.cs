using System.Collections.Generic;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ICallSpecification
    {
        bool IsSatisfiedBy(ICall call);
        string Format(ICall call);
        ICallSpecification CreateCopyThatMatchesAnyArguments();
        void InvokePerArgumentActions(CallInfo callInfo);
        IEnumerable<ArgumentMatchInfo> NonMatchingArguments(ICall call);
    }
}