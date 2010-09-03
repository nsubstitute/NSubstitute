using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    class ArrayContentsArgumentSpecificationFactory : IArrayContentsArgumentSpecificationFactory
    {
        public IArgumentSpecification Create(IEnumerable<IArgumentSpecification> contentsArgumentSpecifications, Type arrayType)
        {
            return new ArrayContentsArgumentSpecification(contentsArgumentSpecifications, arrayType);
        }
    }
}