using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IParamsArgumentSpecificationFactory
    {
        IArgumentSpecification Create(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications);
    }
}