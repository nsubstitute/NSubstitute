using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface IArgumentSpecificationDequeue
    {
        [Obsolete("This method is deprecated and will be removed in future versions of product.")]
        IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(MethodInfo methodInfo);

        IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(int parametersCount);
    }
}