using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IArrayArgumentSpecificationsFactory
    {
        IEnumerable<IArgumentSpecification> Create(object arrayArgument, IEnumerable<IParameterInfo> parameterInfos, ISuppliedArgumentSpecifications suppliedArgumentSpecifications);
    }
}