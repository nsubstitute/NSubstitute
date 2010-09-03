using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class NonParamsArgumentSpecificationFactory : INonParamsArgumentSpecificationFactory
    {
        private readonly IDefaultChecker _defaultChecker;
        private readonly IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;

        public NonParamsArgumentSpecificationFactory(IDefaultChecker defaultChecker, IArgumentEqualsSpecificationFactory argumentEqualsSpecificationFactory)
        {
            _defaultChecker = defaultChecker;
            _argumentEqualsSpecificationFactory = argumentEqualsSpecificationFactory;
        }

        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            if (!_defaultChecker.IsDefault(argument, parameterInfo.ParameterType))
            {
                return _argumentEqualsSpecificationFactory.Create(argument, parameterInfo.ParameterType);
            }
            if (!suppliedArgumentSpecifications.AnyFor(parameterInfo.ParameterType))
            {
                return _argumentEqualsSpecificationFactory.Create(argument, parameterInfo.ParameterType);
            }
            if (suppliedArgumentSpecifications.NextFor(parameterInfo.ParameterType))
            {
                return suppliedArgumentSpecifications.Dequeue();
            }
            throw new AmbiguousArgumentsException();
        }
    }
}