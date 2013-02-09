using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class ParameterInfosFromParamsArrayFactory : IParameterInfosFromParamsArrayFactory
    {
        public IEnumerable<IParameterInfo> Create(object paramsArrayArgument, Type paramsArrayType)
        {
            var elementType = paramsArrayType.GetElementType();
            var arrayElements = ((IEnumerable)paramsArrayArgument).Cast<object>();
            return arrayElements.Select<object, IParameterInfo>(x => new ParameterInfoFromType(elementType));
        }
    }
}