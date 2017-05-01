using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecificationsFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos, MatchArgs matchArgs);
    }
}