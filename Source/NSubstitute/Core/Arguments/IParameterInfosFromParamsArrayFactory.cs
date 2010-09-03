using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IParameterInfosFromParamsArrayFactory
    {
        IEnumerable<IParameterInfo> Create(object paramsArrayArgument, Type paramsArrayType);
    }
}