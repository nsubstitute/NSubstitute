using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        private readonly IParamsArgumentSpecificationFactory _paramsArgumentSpecificationFactory;
        private readonly INonParamsArgumentSpecificationFactory _nonParamsArgumentSpecificationFactory;

        public ArgumentSpecificationFactory(IParamsArgumentSpecificationFactory paramsArgumentSpecificationFactory, INonParamsArgumentSpecificationFactory nonParamsArgumentSpecificationFactory)
        {
            _nonParamsArgumentSpecificationFactory = nonParamsArgumentSpecificationFactory;
            _paramsArgumentSpecificationFactory = paramsArgumentSpecificationFactory;
        }

        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            if (parameterInfo.IsParams)
            {
                return _paramsArgumentSpecificationFactory.Create(argument, parameterInfo, suppliedArgumentSpecifications);
            }
            else
            {
                return _nonParamsArgumentSpecificationFactory.Create(argument, parameterInfo, suppliedArgumentSpecifications);
            }
        }
    }
}