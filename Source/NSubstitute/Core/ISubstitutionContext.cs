using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ISubstitutionContext
    {
        ISubstituteFactory SubstituteFactory { get; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        Query Query { get; }
        void LastCallShouldReturn(IReturn value, MatchArgs matchArgs);
        void LastCallRouter(ICallRouter callRouter);
        ICallRouter GetCallRouterFor(object substitute);
        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();
        void RaiseEventForNextCall(Func<ICall, object[]> getArguments);
    }
}