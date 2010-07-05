using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IMixedArgumentSpecificationFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes);
    }
}