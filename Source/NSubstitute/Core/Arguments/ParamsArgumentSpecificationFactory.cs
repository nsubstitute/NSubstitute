using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ParamsArgumentSpecificationFactory : IParamsArgumentSpecificationFactory
    {
        private readonly IDefaultChecker _defaultChecker;
        private readonly IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;
        private readonly IArrayArgumentSpecificationsFactory _arrayArgumentSpecificationsFactory;
        private readonly IParameterInfosFromParamsArrayFactory _parameterInfosFromParamsArrayFactory;
        private readonly ISuppliedArgumentSpecificationsFactory _suppliedArgumentSpecificationsFactory;
        private readonly IArrayContentsArgumentSpecificationFactory _arrayContentsArgumentSpecificationFactory;

        public ParamsArgumentSpecificationFactory(IDefaultChecker defaultChecker, 
                                                IArgumentEqualsSpecificationFactory argumentEqualsSpecificationFactory, 
                                                IArrayArgumentSpecificationsFactory arrayArgumentSpecificationsFactory,
                                                IParameterInfosFromParamsArrayFactory parameterInfosFromParamsArrayFactory,
                                                ISuppliedArgumentSpecificationsFactory suppliedArgumentSpecificationsFactory,
                                                IArrayContentsArgumentSpecificationFactory arrayContentsArgumentSpecificationFactory)
        {
            _defaultChecker = defaultChecker;
            _argumentEqualsSpecificationFactory = argumentEqualsSpecificationFactory;
            _arrayArgumentSpecificationsFactory = arrayArgumentSpecificationsFactory;
            _parameterInfosFromParamsArrayFactory = parameterInfosFromParamsArrayFactory;
            _suppliedArgumentSpecificationsFactory = suppliedArgumentSpecificationsFactory;
            _arrayContentsArgumentSpecificationFactory = arrayContentsArgumentSpecificationFactory;
        }

        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            if (_defaultChecker.IsDefault(argument, parameterInfo.ParameterType))
            {
                if (suppliedArgumentSpecifications.NextFor(parameterInfo.ParameterType))
                {
                    var argumentSpecification = suppliedArgumentSpecifications.Dequeue();
                    if (suppliedArgumentSpecifications.DequeueAll().Count() == 0)
                    {
                        return argumentSpecification;
                    }
                }
                else
                {
                    if (!suppliedArgumentSpecifications.AnyFor(parameterInfo.ParameterType))
                    {
                        if (suppliedArgumentSpecifications.DequeueAll().Count() == 0)
                        {
                            return _argumentEqualsSpecificationFactory.Create(argument, parameterInfo.ParameterType);
                        }
                    }
                }
            }
            else
            {
                var paramterInfosFromParamsArray = _parameterInfosFromParamsArrayFactory.Create(argument, parameterInfo.ParameterType);
                var suppliedArgumentSpecificationsFromParamsArray = _suppliedArgumentSpecificationsFactory.Create(suppliedArgumentSpecifications.DequeueAll());
                var arrayArgumentSpecifications = _arrayArgumentSpecificationsFactory.Create(argument, paramterInfosFromParamsArray, suppliedArgumentSpecificationsFromParamsArray);
                return _arrayContentsArgumentSpecificationFactory.Create(arrayArgumentSpecifications, parameterInfo.ParameterType);
            }
            throw new AmbiguousArgumentsException();
        }

    }
}