using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IArrayContentsArgumentSpecificationFactory
    {
        IArgumentSpecification Create(IEnumerable<IArgumentSpecification> contentsArgumentSpecifications, Type arrayType);
    }
}