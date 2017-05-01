using System;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentEqualsSpecificationFactory
    {
        IArgumentSpecification Create(object value, Type forType);
    }
}