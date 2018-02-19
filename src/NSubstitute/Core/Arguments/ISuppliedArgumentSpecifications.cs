using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface ISuppliedArgumentSpecifications
    {
        IEnumerable<IArgumentSpecification> AllSpecifications { get; }
        bool AnyFor(object argument, Type argumentType);
        bool IsNextFor(object argument, Type argumentType);
        IArgumentSpecification Dequeue();
        IEnumerable<IArgumentSpecification> DequeueRemaining();
    }
}