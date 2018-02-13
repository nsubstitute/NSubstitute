using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface IArgumentSpecificationDequeue
    {
        IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(MethodInfo methodInfo);
    }
}