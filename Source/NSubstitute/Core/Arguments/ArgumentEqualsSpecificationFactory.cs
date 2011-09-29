using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentEqualsSpecificationFactory : IArgumentEqualsSpecificationFactory
    {
        public IArgumentSpecification Create(object value, Type forType)
        {
            return new ArgumentSpecification(forType, new EqualsArgumentMatcher(value));
        }
    }
}