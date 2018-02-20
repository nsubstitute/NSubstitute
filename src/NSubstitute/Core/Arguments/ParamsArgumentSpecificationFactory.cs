using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ParamsArgumentSpecificationFactory : IParamsArgumentSpecificationFactory
    {
        private readonly IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;
        private readonly IArrayArgumentSpecificationsFactory _arrayArgumentSpecificationsFactory;
        private readonly IParameterInfosFromParamsArrayFactory _parameterInfosFromParamsArrayFactory;
        private readonly IArrayContentsArgumentSpecificationFactory _arrayContentsArgumentSpecificationFactory;

        public ParamsArgumentSpecificationFactory(IArgumentEqualsSpecificationFactory argumentEqualsSpecificationFactory,
                                                IArrayArgumentSpecificationsFactory arrayArgumentSpecificationsFactory,
                                                IParameterInfosFromParamsArrayFactory parameterInfosFromParamsArrayFactory,
                                                IArrayContentsArgumentSpecificationFactory arrayContentsArgumentSpecificationFactory)
        {
            _argumentEqualsSpecificationFactory = argumentEqualsSpecificationFactory;
            _arrayArgumentSpecificationsFactory = arrayArgumentSpecificationsFactory;
            _parameterInfosFromParamsArrayFactory = parameterInfosFromParamsArrayFactory;
            _arrayContentsArgumentSpecificationFactory = arrayContentsArgumentSpecificationFactory;
        }

        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            // Next specificaion is for the whole params array.
            if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
            {
                return suppliedArgumentSpecifications.Dequeue();
            }

            // Check whether the specification ambiguity could happen.
            bool isAmbiguousSpecificationPresent = suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType);
            if (isAmbiguousSpecificationPresent)
            {
                throw new AmbiguousArgumentsException(suppliedArgumentSpecifications.AllSpecifications);
            }

            // User passed "null" as the params array value.
            if (argument == null)
            {
                return _argumentEqualsSpecificationFactory.Create(null, parameterInfo.ParameterType);
            }

            // User specified arguments using the native params syntax.
            var paramterInfosFromParamsArray = _parameterInfosFromParamsArrayFactory.Create(argument, parameterInfo.ParameterType);
            var arrayArgumentSpecifications = _arrayArgumentSpecificationsFactory.Create(argument, paramterInfosFromParamsArray, suppliedArgumentSpecifications);
            return _arrayContentsArgumentSpecificationFactory.Create(arrayArgumentSpecifications, parameterInfo.ParameterType);
        }
    }
}