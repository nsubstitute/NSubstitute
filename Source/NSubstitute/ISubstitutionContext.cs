using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn<T>(T value);
        void LastCallRouter(ICallRouter callRouter);
        ISubstituteFactory GetSubstituteFactory();
        ICallRouter GetCallRouterFor(object substitute);
        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();
    }
}