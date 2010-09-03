using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface ISuppliedArgumentSpecifications
    {
        bool AnyFor(Type type);
        bool NextFor(Type type);
        IArgumentSpecification Dequeue();
        IEnumerable<IArgumentSpecification> DequeueAll();
    }
}