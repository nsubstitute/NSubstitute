using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class NonParamsArgumentSpecificationFactory : INonParamsArgumentSpecificationFactory
    {
        private readonly IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;

        public NonParamsArgumentSpecificationFactory(IArgumentEqualsSpecificationFactory argumentEqualsSpecificationFactory)
        {
            _argumentEqualsSpecificationFactory = argumentEqualsSpecificationFactory;
        }

        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
            {
                return suppliedArgumentSpecifications.Dequeue();
            }
            if (!suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType) || parameterInfo.IsOptional || parameterInfo.IsOut)
            {
                return _argumentEqualsSpecificationFactory.Create(argument, parameterInfo.ParameterType);
            }
            throw new AmbiguousArgumentsException();
        }
    }
}