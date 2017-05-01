using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IMixedArgumentSpecificationsFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos);
    }
}