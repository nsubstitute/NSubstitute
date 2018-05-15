using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public interface IMixedArgumentSpecificationsFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, IParameterInfo[] parameterInfos, MethodInfo methodInfo);
    }
}