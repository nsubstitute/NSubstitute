using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core;

public class ArgumentSpecificationDequeue(Func<IList<IArgumentSpecification>> dequeueAllQueuedArgSpecs) : IArgumentSpecificationDequeue
{
    private static readonly IArgumentSpecification[] EmptySpecifications = [];

    public IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(int parametersCount)
    {
        if (parametersCount == 0)
        {
            // We violate public contract, as mutable list was expected as result.
            // However, in reality we never expect value to be mutated, so this optimization is fine.
            // We are not allowed to change public contract due to SemVer, so keeping that as it is.
            return EmptySpecifications;
        }

        var queuedArgSpecifications = dequeueAllQueuedArgSpecs.Invoke();
        return queuedArgSpecifications;
    }

    public IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(MethodInfo methodInfo)
    {
        return DequeueAllArgumentSpecificationsForMethod(methodInfo.GetParameters().Length);
    }
}