using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface ISubstitutionContext
    {
        ISubstituteFactory SubstituteFactory { get; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        PendingSpecificationInfo PendingSpecificationInfo { get; set; }
        ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs);
        void LastCallRouter(ICallRouter callRouter);
        ICallRouter GetCallRouterFor(object substitute);
        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();
        void RaiseEventForNextCall(Func<ICall, object[]> getArguments);
        IQueryResults RunQuery(Action calls);
        bool IsQuerying { get; }
        void AddToQuery(object target, ICallSpecification callSpecification);
        void ClearLastCallRouter();
        IRouteFactory GetRouteFactory();
    }
}