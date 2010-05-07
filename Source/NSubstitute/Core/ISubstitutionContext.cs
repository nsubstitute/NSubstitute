using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn<T>(T value);
        void LastCallRouter(ICallRouter callRouter);
        ISubstituteFactory GetSubstituteFactory();
        ICallRouter GetCallRouterFor(object substitute);
        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();
        void RaiseEventForNextCall(Func<ICall, object[]> getArguments);
    }
}