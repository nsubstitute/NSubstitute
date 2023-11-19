using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecificationsFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object?[] arguments, IParameterInfo[] parameterInfos, MethodInfo methodInfo, MatchArgs matchArgs);
    }
}