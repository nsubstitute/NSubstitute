using NSubstitute.Core.Arguments;

namespace NSubstitute.Core;

public interface IArgumentSpecificationDequeue
{
    IList<IArgumentSpecification> DequeueAllArgumentSpecificationsForMethod(int parametersCount);
}