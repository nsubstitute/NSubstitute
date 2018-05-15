using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ArrayArgumentSpecificationsFactory : IArrayArgumentSpecificationsFactory
    {
        private readonly INonParamsArgumentSpecificationFactory _nonParamsArgumentSpecificationFactory;

        public ArrayArgumentSpecificationsFactory(INonParamsArgumentSpecificationFactory nonParamsArgumentSpecificationFactory)
        {
            _nonParamsArgumentSpecificationFactory = nonParamsArgumentSpecificationFactory;
        }

        public IEnumerable<IArgumentSpecification> Create(object arrayArgument, IEnumerable<IParameterInfo> parameterInfos, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            var arrayMembers = (IEnumerable)arrayArgument;
            var index = 0;
            var result = new List<IArgumentSpecification>();
            foreach (var member in arrayMembers)
            {
                try
                {
                    result.Add(_nonParamsArgumentSpecificationFactory.Create(member, parameterInfos.ElementAt(index), suppliedArgumentSpecifications));
                }
                catch (AmbiguousArgumentsException ex)
                {
                    ex.Data[AmbiguousArgumentsException.NonReportedResolvedSpecificationsKey] = result;
                    throw;
                }
                index++;
            }
            return result;
        }
    }
}