using NSubstitute.Core.Arguments;

namespace NSubstitute.Internal.Core.Arguments;

public class SuppliedArgumentSpecificationsFactory(IArgumentSpecificationCompatibilityTester argumentSpecificationCompatTester) : ISuppliedArgumentSpecificationsFactory
{
    public ISuppliedArgumentSpecifications Create(IEnumerable<IArgumentSpecification> argumentSpecifications)
    {
        return new SuppliedArgumentSpecifications(argumentSpecificationCompatTester, argumentSpecifications);
    }
}